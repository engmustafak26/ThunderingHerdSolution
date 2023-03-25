using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using ThunderingHerdSolution.Core.Abstractions;
using ThunderingHerdSolution.Core.Constants;
using ThunderingHerdSolution.Core.Domain;
using ThunderingHerdSolution.Core.Events;

namespace ThunderingHerdSolution.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICache _cache;
        private readonly IMessageBroker _messageBroker;

        public WeatherForecastController(IConfiguration configuration, ICache cache, IMessageBroker messageBroker)
        {
            _configuration = configuration;
            _cache = cache;
            _messageBroker = messageBroker;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var eventHandler = new ThirdPartyApiRequestingEventHandler();
            string cacheKey = eventHandler.CacheKey;


            var cachedObject = await _cache.GetAsync<WeatherForecast[]>(cacheKey);
            if (cachedObject != null)
                return cachedObject;

            using (ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(_configuration["Redis:ReplicasConnectionString"]))
            {
                eventHandler.ExpirationTime = _configuration.GetValue<TimeSpan>("WeatherApiCacheExpiration");

                TaskCompletionSource _waitingSource = new();
                Task timeOutTask = Task.Delay(_configuration.GetValue<int>("WeatherApitaskTimeOutInMilliSeconds"));

                var pubsub = redisConnection.GetSubscriber();
                pubsub.Subscribe(AppConstants.CachingJobPublishChannel, (channel, jsonData) =>
                {
                    _waitingSource.SetResult();
                }, CommandFlags.FireAndForget);

                await _messageBroker.PublishAsync(AppConstants.ApiPublishChannel, eventHandler);

                var returnedTask = await Task.WhenAny(_waitingSource.Task, timeOutTask);
                if (returnedTask == timeOutTask)
                {
                    return new WeatherForecast[0];
                }

                return await _cache.GetAsync<WeatherForecast[]>(cacheKey);


            }
        }
    }
}


