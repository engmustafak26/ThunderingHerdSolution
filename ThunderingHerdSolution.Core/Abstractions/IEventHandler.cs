using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThunderingHerdSolution.Core.Abstractions
{
    public interface IEventHandler
    {
        string ConcreteTypeAssemblyQualifiedName { get; }
        string CacheKey { get; }
        TimeSpan ExpirationTime { get; set; }
        string EventBackName { get; }
        Task<(string Response, Exception Exception)> Handle(IServiceProvider serviceProvider);
    }
}
