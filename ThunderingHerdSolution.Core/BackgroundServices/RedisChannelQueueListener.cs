using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThunderingHerdSolution.Core.Abstractions;
using ThunderingHerdSolution.Core.Constants;

namespace ThunderingHerdSolution.Core.BackgroundServices
{
    public class RedisChannelQueueListener : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly TaskCompletionSource _waitingSource = new();
        private readonly IJobRescheduler _jobRescheduler;
        private readonly SemaphoreSlim _semaphoregate = new SemaphoreSlim(1);
        private DateTime eventExpirationDateTime;


        public RedisChannelQueueListener(IConfiguration configuration, IJobRescheduler jobRescheduler)
        {
            _configuration = configuration;
            _jobRescheduler = jobRescheduler;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            try
            {

                using (ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(_configuration["Redis:ReplicasConnectionString"]))
                {
                    var pubsub = redisConnection.GetSubscriber();

                    pubsub.Subscribe(AppConstants.ApiPublishChannel, async (channel, jsonData) =>
                    {

                        await _semaphoregate.WaitAsync();
                        if (DateTime.Now >= eventExpirationDateTime)
                        {
                            eventExpirationDateTime = await _jobRescheduler.InvokeEventHandlerAsync(ConvertFromJson(jsonData), publishToOwnChannel: true);
                            _semaphoregate.Release();

                        }
                        else
                        {
                            _semaphoregate.Release();
                            return;
                        }
                    });

                    stoppingToken.Register(() => _waitingSource.SetResult());
                    await _waitingSource.Task.ConfigureAwait(false);
                }
            }
            catch { }

        }

        private static IEventHandler ConvertFromJson(string json)
        {

            var cacheFetchApiLogicConcreteType = JsonSerializer.Deserialize<CacheFetchApiLogicConcreteType>(json);
            Type type = Type.GetType(cacheFetchApiLogicConcreteType.ConcreteTypeAssemblyQualifiedName);
            return JsonSerializer.Deserialize(json, type) as IEventHandler;
        }
        class CacheFetchApiLogicConcreteType
        {
            public string ConcreteTypeAssemblyQualifiedName { get; set; }
        }
    }
}
