using System.Diagnostics;
using System.Reflection.Emit;
using System.Text.Json;
using ThunderingHerdSolution.Core.Abstractions;
using ThunderingHerdSolution.Core.Constants;

namespace ThunderingHerdSolution.CachingJob
{
    public class JobRescheduler : IJobRescheduler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageBroker _messageBroker;
        private readonly ICache _cache;

        public JobRescheduler(IServiceProvider serviceProvider, ICache cache, IMessageBroker messageBroker)
        {
            _serviceProvider = serviceProvider;
            _cache = cache;
            _messageBroker = messageBroker;
        }

        public async Task<DateTime> InvokeEventHandlerAsync(IEventHandler eventHandler, bool publishToOwnChannel = false)
        {

            try
            {
                var handlerResponse = await eventHandler.Handle(_serviceProvider);

                if (handlerResponse.Exception is not null)
                    throw handlerResponse.Exception;

                await _cache.SetAsync(eventHandler.CacheKey, handlerResponse.Response, eventHandler.ExpirationTime);


                if (publishToOwnChannel)
                {
                    await _messageBroker.PublishAsync(AppConstants.CachingJobPublishChannel, eventHandler.EventBackName).ConfigureAwait(false);
                }

                return DateTime.Now.Add(eventHandler.ExpirationTime);
            }
            catch (Exception ex)
            {
                string error = $"{DateTime.Now} - {eventHandler.ToString()} => {ex.ToString()}";
                Console.WriteLine(error);
                Debug.WriteLine(error);
                return DateTime.MinValue;
            }

        }

    }

}
