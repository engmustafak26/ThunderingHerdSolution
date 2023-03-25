using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using ThunderingHerdSolution.Core.Abstractions;
using static ThunderingHerdSolution.Core.Constants.AppConstants;
using Microsoft.Extensions.Logging;
using ThunderingHerdSolution.Core.Domain;
using ThunderingHerdSolution.Core.Constants;

namespace ThunderingHerdSolution.Core.Events
{
    public class ThirdPartyApiRequestingEventHandler : IEventHandler
    {
        [JsonInclude]
        public string ConcreteTypeAssemblyQualifiedName { get; private set; }
        [JsonInclude]
        public string CacheKey { get; private set; } = AppConstants.WeatherApiCacheKey;
        [JsonInclude]
        public TimeSpan ExpirationTime { get; set; }
        [JsonInclude]
        public string EventBackName { get; private set; } = ThirdPartyApiRequestCompletedEvent;



        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        public ThirdPartyApiRequestingEventHandler() => ConcreteTypeAssemblyQualifiedName = this.GetType().AssemblyQualifiedName;

        public async Task<(string Response, Exception Exception)> Handle(IServiceProvider serviceProvider)
        {
            try
            {
                // we can use service builder for other code activities as query a generic repository
                using (var scope = serviceProvider.CreateScope())
                {

                    var response = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    }).ToArray();


                    return (JsonSerializer.Serialize(response), null);
                }
            }
            catch (Exception ex)
            {
                return (string.Empty, ex);

            }
        }


    }
}


