using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThunderingHerdSolution.Core.Abstractions;

namespace ThunderingHerdSolution.Core.Implementations
{
    public class RedisBroker : IMessageBroker
    {
        private readonly IConfiguration _configuration;

        public RedisBroker(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PublishAsync<T>(string channel, T data)
        {
            try
            {
                _ = Task.Run(async () =>
                {
                    using (ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(_configuration["Redis:ConnectionString"]))
                    {
                        var pubsub = redisConnection.GetSubscriber();
                        var jsonData = JsonSerializer.Serialize(data);
                        await pubsub.PublishAsync(channel, jsonData, CommandFlags.FireAndForget);
                    }
                });

            }
            catch { }
        }

        public async Task PublishAsync(string channel, string rawData)
        {
            try
            {
                _ = Task.Run(async () =>
                {
                    using (ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(_configuration["Redis:ConnectionString"]))
                    {
                        var pubsub = redisConnection.GetSubscriber();
                        await pubsub.PublishAsync(channel, rawData, CommandFlags.FireAndForget);
                    }
                });

            }
            catch { }
        }
    }
}
