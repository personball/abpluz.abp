using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Volo.Abp.Caching.StackExchageRedis
{
    internal static class AbpluzAbpRedisExtensions
    {
        internal static RedisValue[][] HashMemberGetMany(
            this IDatabase cache,
            string[] keys,
            params string[] members)
        {
            var tasks = new Task<RedisValue[]>[keys.Length];
            var fields = members.Select(member => (RedisValue)member).ToArray();
            var results = new RedisValue[keys.Length][];

            for (var i = 0; i < keys.Length; i++)
            {
                tasks[i] = cache.HashGetAsync((RedisKey)keys[i], fields);
            }

            for (var i = 0; i < tasks.Length; i++)
            {
                results[i] = cache.Wait(tasks[i]);
            }

            return results;
        }

        internal static async Task<RedisValue[][]> HashMemberGetManyAsync(
            this IDatabase cache,
            string[] keys,
            params string[] members)
        {
            var tasks = new Task<RedisValue[]>[keys.Length];
            var fields = members.Select(member => (RedisValue)member).ToArray();

            for (var i = 0; i < keys.Length; i++)
            {
                tasks[i] = cache.HashGetAsync((RedisKey)keys[i], fields);
            }

            return await Task.WhenAll(tasks);
        }

        internal static RedisValue[] HashMemberGet(this IDatabase cache, string key, params string[] members)
        {
            // TODO: Error checking?
            return cache.HashGet(key, GetRedisMembers(members));
        }

        internal static async Task<RedisValue[]> HashMemberGetAsync(
            this IDatabase cache,
            string key,
            params string[] members)
        {
            // TODO: Error checking?
            return await cache.HashGetAsync(key, GetRedisMembers(members)).ConfigureAwait(false);
        }

        private static RedisValue[] GetRedisMembers(params string[] members)
        {
            var redisMembers = new RedisValue[members.Length];
            for (int i = 0; i < members.Length; i++)
            {
                redisMembers[i] = (RedisValue)members[i];
            }

            return redisMembers;
        }
    }
}
