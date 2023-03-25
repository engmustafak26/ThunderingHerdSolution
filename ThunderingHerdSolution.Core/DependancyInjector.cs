using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderingHerdSolution.Core.Abstractions;
using ThunderingHerdSolution.Core.Implementations;

namespace ThunderingHerdSolution.Core
{
    public static class DependancyInjector
    {
        public static IServiceCollection AddCoreModule(this IServiceCollection services, IConfiguration configuration, Action customAction = null)
        {


            try
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration["Redis:ConnectionString"];
                    options.InstanceName = configuration["Redis:ChannelPrefix"];
                });
                var redisMaster = services.BuildServiceProvider().GetRequiredService<IDistributedCache>();

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration["Redis:ReplicasConnectionString"];
                    options.InstanceName = configuration["Redis:ChannelPrefix"];
                });
                var redisSlave = services.BuildServiceProvider().GetRequiredService<IDistributedCache>();

                services.AddSingleton<ICache>(_cache => new RedisCache(redisMaster, redisSlave));
            }
            catch { }

            services.AddSingleton<IMessageBroker, RedisBroker>();

            if (customAction is not null)
                customAction();

            return services;
        }

    }
}
