using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Caching.StackExchageRedis
{

    /// <summary>
    /// see https://github.com/dotnet/extensions/blob/master/src/Caching/StackExchangeRedis/src/RedisCache.cs
    /// see https://github.com/abpframework/abp/blob/dev/framework/src/Volo.Abp.Caching.StackExchangeRedis/Volo/Abp/Caching/StackExchangeRedis/AbpRedisCache.cs
    /// </summary>
    [DisableConventionalRegistration]
    public class AbpluzAbpRedisCache : IDistributedCache, ICacheSupportsMultipleItems, IDisposable
    {
        // KEYS[1] = = key
        // ARGV[1] = absolute-expiration - ticks as long (-1 for none)
        // ARGV[2] = sliding-expiration - ticks as long (-1 for none)
        // ARGV[3] = relative-expiration (long, in seconds, -1 for none) - Min(absolute-expiration - Now, sliding-expiration)
        // ARGV[4] = data - byte[]
        // this order should not change LUA script depends on it
        private const string SetScript = (@"
                redis.call('HMSET', KEYS[1], 'absexp', ARGV[1], 'sldexp', ARGV[2], 'data', ARGV[4])
                if ARGV[3] ~= '-1' then
                  redis.call('EXPIRE', KEYS[1], ARGV[3])
                end
                return 1");
        private const string AbsoluteExpirationKey = "absexp";
        private const string SlidingExpirationKey = "sldexp";
        private const string DataKey = "data";
        private const long NotPresent = -1;

        private volatile ConnectionMultiplexer _connection;
        private IDatabase _cache;
        private bool _disposed;

        private readonly RedisCacheOptions _options;
        private readonly string _instance;

        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        public AbpluzAbpRedisCache(IOptions<RedisCacheOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = optionsAccessor.Value;

            // This allows partitioning a single backend cache for use with multiple apps/services.
            _instance = _options.InstanceName ?? string.Empty;
        }

        public byte[] Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return GetAndRefresh(key, getData: true);
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            token.ThrowIfCancellationRequested();

            return await GetAndRefreshAsync(key, getData: true, token: token).ConfigureAwait(false);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Connect();

            var creationTime = DateTimeOffset.UtcNow;

            var absoluteExpiration = GetAbsoluteExpiration(creationTime, options);

            var result = _cache.ScriptEvaluate(SetScript, new RedisKey[] { _instance + key },
                new RedisValue[]
                {
                        absoluteExpiration?.Ticks ?? NotPresent,
                        options.SlidingExpiration?.Ticks ?? NotPresent,
                        GetExpirationInSeconds(creationTime, absoluteExpiration, options) ?? NotPresent,
                        value
                }, CommandFlags.DemandMaster);
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            token.ThrowIfCancellationRequested();

            await ConnectAsync(token).ConfigureAwait(false);

            var creationTime = DateTimeOffset.UtcNow;

            var absoluteExpiration = GetAbsoluteExpiration(creationTime, options);

            await _cache.ScriptEvaluateAsync(SetScript, new RedisKey[] { _instance + key },
                new RedisValue[]
                {
                        absoluteExpiration?.Ticks ?? NotPresent,
                        options.SlidingExpiration?.Ticks ?? NotPresent,
                        GetExpirationInSeconds(creationTime, absoluteExpiration, options) ?? NotPresent,
                        value
                }, CommandFlags.DemandMaster).ConfigureAwait(false);
        }

        public void Refresh(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            GetAndRefresh(key, getData: false);
        }

        public async Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            token.ThrowIfCancellationRequested();

            await GetAndRefreshAsync(key, getData: false, token: token).ConfigureAwait(false);
        }

        private void Connect()
        {
            CheckDisposed();
            if (_cache != null)
            {
                return;
            }

            _connectionLock.Wait();
            try
            {
                if (_cache == null)
                {
                    if (_options.ConfigurationOptions != null)
                    {
                        _connection = ConnectionMultiplexer.Connect(_options.ConfigurationOptions);
                    }
                    else
                    {
                        _connection = ConnectionMultiplexer.Connect(_options.Configuration);
                    }
                    _cache = _connection.GetDatabase();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        private async Task ConnectAsync(CancellationToken token = default(CancellationToken))
        {
            CheckDisposed();
            token.ThrowIfCancellationRequested();

            if (_cache != null)
            {
                return;
            }

            await _connectionLock.WaitAsync(token).ConfigureAwait(false);
            try
            {
                if (_cache == null)
                {
                    if (_options.ConfigurationOptions != null)
                    {
                        _connection = await ConnectionMultiplexer.ConnectAsync(_options.ConfigurationOptions).ConfigureAwait(false);
                    }
                    else
                    {
                        _connection = await ConnectionMultiplexer.ConnectAsync(_options.Configuration).ConfigureAwait(false);
                    }

                    _cache = _connection.GetDatabase();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        private byte[] GetAndRefresh(string key, bool getData)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            Connect();

            // This also resets the LRU status as desired.
            // TODO: Can this be done in one operation on the server side? Probably, the trick would just be the DateTimeOffset math.
            RedisValue[] results;
            if (getData)
            {
                results = _cache.HashMemberGet(_instance + key, AbsoluteExpirationKey, SlidingExpirationKey, DataKey);
            }
            else
            {
                results = _cache.HashMemberGet(_instance + key, AbsoluteExpirationKey, SlidingExpirationKey);
            }

            // TODO: Error handling
            if (results.Length >= 2)
            {
                MapMetadata(results, out DateTimeOffset? absExpr, out TimeSpan? sldExpr);
                Refresh(key, absExpr, sldExpr);
            }

            if (results.Length >= 3 && results[2].HasValue)
            {
                return results[2];
            }

            return null;
        }

        private async Task<byte[]> GetAndRefreshAsync(string key, bool getData, CancellationToken token = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            token.ThrowIfCancellationRequested();

            await ConnectAsync(token).ConfigureAwait(false);

            // This also resets the LRU status as desired.
            // TODO: Can this be done in one operation on the server side? Probably, the trick would just be the DateTimeOffset math.
            RedisValue[] results;
            if (getData)
            {
                results = await _cache.HashMemberGetAsync(_instance + key, AbsoluteExpirationKey, SlidingExpirationKey, DataKey).ConfigureAwait(false);
            }
            else
            {
                results = await _cache.HashMemberGetAsync(_instance + key, AbsoluteExpirationKey, SlidingExpirationKey).ConfigureAwait(false);
            }

            // TODO: Error handling
            if (results.Length >= 2)
            {
                MapMetadata(results, out DateTimeOffset? absExpr, out TimeSpan? sldExpr);
                await RefreshAsync(key, absExpr, sldExpr, token).ConfigureAwait(false);
            }

            if (results.Length >= 3 && results[2].HasValue)
            {
                return results[2];
            }

            return null;
        }

        public void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            Connect();

            _cache.KeyDelete(_instance + key, CommandFlags.DemandMaster);
            // TODO: Error handling
        }

        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            await ConnectAsync(token).ConfigureAwait(false);

            await _cache.KeyDeleteAsync(_instance + key, CommandFlags.DemandMaster).ConfigureAwait(false);
            // TODO: Error handling
        }

        private void MapMetadata(RedisValue[] results, out DateTimeOffset? absoluteExpiration, out TimeSpan? slidingExpiration)
        {
            absoluteExpiration = null;
            slidingExpiration = null;
            var absoluteExpirationTicks = (long?)results[0];
            if (absoluteExpirationTicks.HasValue && absoluteExpirationTicks.Value != NotPresent)
            {
                absoluteExpiration = new DateTimeOffset(absoluteExpirationTicks.Value, TimeSpan.Zero);
            }
            var slidingExpirationTicks = (long?)results[1];
            if (slidingExpirationTicks.HasValue && slidingExpirationTicks.Value != NotPresent)
            {
                slidingExpiration = new TimeSpan(slidingExpirationTicks.Value);
            }
        }

        private void Refresh(string key, DateTimeOffset? absExpr, TimeSpan? sldExpr)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // Note Refresh has no effect if there is just an absolute expiration (or neither).
            TimeSpan? expr = null;
            if (sldExpr.HasValue)
            {
                if (absExpr.HasValue)
                {
                    var relExpr = absExpr.Value - DateTimeOffset.Now;
                    expr = relExpr <= sldExpr.Value ? relExpr : sldExpr;
                }
                else
                {
                    expr = sldExpr;
                }
                _cache.KeyExpire(_instance + key, expr, CommandFlags.DemandMaster);
                // TODO: Error handling
            }
        }

        private async Task RefreshAsync(string key, DateTimeOffset? absExpr, TimeSpan? sldExpr, CancellationToken token = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            token.ThrowIfCancellationRequested();

            // Note Refresh has no effect if there is just an absolute expiration (or neither).
            TimeSpan? expr = null;
            if (sldExpr.HasValue)
            {
                if (absExpr.HasValue)
                {
                    var relExpr = absExpr.Value - DateTimeOffset.Now;
                    expr = relExpr <= sldExpr.Value ? relExpr : sldExpr;
                }
                else
                {
                    expr = sldExpr;
                }
                await _cache.KeyExpireAsync(_instance + key, expr, CommandFlags.DemandMaster).ConfigureAwait(false);
                // TODO: Error handling
            }
        }

        private static long? GetExpirationInSeconds(DateTimeOffset creationTime, DateTimeOffset? absoluteExpiration, DistributedCacheEntryOptions options)
        {
            if (absoluteExpiration.HasValue && options.SlidingExpiration.HasValue)
            {
                return (long)Math.Min(
                    (absoluteExpiration.Value - creationTime).TotalSeconds,
                    options.SlidingExpiration.Value.TotalSeconds);
            }
            else if (absoluteExpiration.HasValue)
            {
                return (long)(absoluteExpiration.Value - creationTime).TotalSeconds;
            }
            else if (options.SlidingExpiration.HasValue)
            {
                return (long)options.SlidingExpiration.Value.TotalSeconds;
            }
            return null;
        }

        private static DateTimeOffset? GetAbsoluteExpiration(DateTimeOffset creationTime, DistributedCacheEntryOptions options)
        {
            if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(DistributedCacheEntryOptions.AbsoluteExpiration),
                    options.AbsoluteExpiration.Value,
                    "The absolute expiration value must be in the future.");
            }

            if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                return creationTime + options.AbsoluteExpirationRelativeToNow;
            }

            return options.AbsoluteExpiration;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _connection?.Close();
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }


        #region ICacheSupportsMultipleItems


        public byte[][] GetMany(
            IEnumerable<string> keys)
        {
            keys = Check.NotNull(keys, nameof(keys));

            return GetAndRefreshMany(keys, true);
        }

        public async Task<byte[][]> GetManyAsync(
            IEnumerable<string> keys,
            CancellationToken token = default)
        {
            keys = Check.NotNull(keys, nameof(keys));

            return await GetAndRefreshManyAsync(keys, true, token);
        }

        public void SetMany(
            IEnumerable<KeyValuePair<string, byte[]>> items,
            DistributedCacheEntryOptions options)
        {
            Connect();

            Task.WaitAll(PipelineSetMany(items, options));
        }

        public async Task SetManyAsync(
            IEnumerable<KeyValuePair<string, byte[]>> items,
            DistributedCacheEntryOptions options,
            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            await ConnectAsync(token);

            await Task.WhenAll(PipelineSetMany(items, options));
        }

        protected virtual byte[][] GetAndRefreshMany(
            IEnumerable<string> keys,
            bool getData)
        {
            Connect();

            var keyArray = keys.Select(key => _instance + key).ToArray();
            RedisValue[][] results;

            if (getData)
            {
                results = _cache.HashMemberGetMany(keyArray, AbsoluteExpirationKey,
                    SlidingExpirationKey, DataKey);
            }
            else
            {
                results = _cache.HashMemberGetMany(keyArray, AbsoluteExpirationKey,
                    SlidingExpirationKey);
            }

            Task.WaitAll(PipelineRefreshManyAndOutData(keyArray, results, out var bytes));

            return bytes;
        }

        protected virtual async Task<byte[][]> GetAndRefreshManyAsync(
            IEnumerable<string> keys,
            bool getData,
            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            await ConnectAsync(token);

            var keyArray = keys.Select(key => _instance + key).ToArray();
            RedisValue[][] results;

            if (getData)
            {
                results = await _cache.HashMemberGetManyAsync(keyArray, AbsoluteExpirationKey,
                    SlidingExpirationKey, DataKey);
            }
            else
            {
                results = await _cache.HashMemberGetManyAsync(keyArray, AbsoluteExpirationKey,
                    SlidingExpirationKey);
            }

            await Task.WhenAll(PipelineRefreshManyAndOutData(keyArray, results, out var bytes));

            return bytes;
        }

        protected virtual Task[] PipelineRefreshManyAndOutData(
            string[] keys,
            RedisValue[][] results,
            out byte[][] bytes)
        {
            bytes = new byte[keys.Length][];
            var tasks = new Task[keys.Length];

            for (var i = 0; i < keys.Length; i++)
            {
                if (results[i].Length >= 2)
                {
                    MapMetadata(results[i], out DateTimeOffset? absExpr, out TimeSpan? sldExpr);

                    if (sldExpr.HasValue)
                    {
                        TimeSpan? expr;

                        if (absExpr.HasValue)
                        {
                            var relExpr = absExpr.Value - DateTimeOffset.Now;
                            expr = relExpr <= sldExpr.Value ? relExpr : sldExpr;
                        }
                        else
                        {
                            expr = sldExpr;
                        }

                        tasks[i] = _cache.KeyExpireAsync(keys[i], expr, CommandFlags.DemandMaster);
                    }
                    else
                    {
                        tasks[i] = Task.CompletedTask;
                    }
                }

                if (results[i].Length >= 3 && results[i][2].HasValue)
                {
                    bytes[i] = results[i][2];
                }
                else
                {
                    bytes[i] = null;
                }
            }

            return tasks;
        }

        protected virtual Task[] PipelineSetMany(
            IEnumerable<KeyValuePair<string, byte[]>> items,
            DistributedCacheEntryOptions options)
        {
            items = Check.NotNull(items, nameof(items));
            options = Check.NotNull(options, nameof(options));

            var itemArray = items.ToArray();
            var tasks = new Task[itemArray.Length];
            var creationTime = DateTimeOffset.UtcNow;
            var absoluteExpiration = GetAbsoluteExpiration(creationTime, options);

            for (var i = 0; i < itemArray.Length; i++)
            {
                tasks[i] = _cache.ScriptEvaluateAsync(SetScript, new RedisKey[] { _instance + itemArray[i].Key },
                    new RedisValue[]
                    {
                        absoluteExpiration?.Ticks ?? NotPresent,
                        options.SlidingExpiration?.Ticks ?? NotPresent,
                        GetExpirationInSeconds(creationTime, absoluteExpiration, options) ?? NotPresent,
                        itemArray[i].Value
                    }, CommandFlags.DemandMaster);
            }

            return tasks;
        }

        #endregion
    }
}
