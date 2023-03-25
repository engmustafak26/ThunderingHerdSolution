using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ThunderingHerdSolution.Core.Constants
{
    public static class AppConstants
    {
        public const string WeatherApiCacheKey = "weather-api-cache-key";
        public const string ApiPublishChannel = "api-publish-channel";
        public const string CachingJobPublishChannel = "caching-job-publish-channel";
        public const string ThirdPartyApiRequestCompletedEvent = "3-party API Request completed";
        
    }
}
