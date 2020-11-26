using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Modularity;

namespace Volo.Abp.Caching.StackExchageRedis
{
    [DependsOn(typeof(AbpCachingModule))]
    public class AbpluzAbpCachingStackExchageRedisModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            context.Services.AddStackExchangeRedisCache(options =>
            {
                var redisConfiguration = configuration["Redis:Configuration"];
                if (!redisConfiguration.IsNullOrEmpty())
                {
                    options.Configuration = configuration["Redis:Configuration"];
                }
            });

            context.Services.Replace(ServiceDescriptor.Singleton<IDistributedCache, AbpluzAbpRedisCache>());
        }
    }
}
