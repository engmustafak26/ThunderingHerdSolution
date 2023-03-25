using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThunderingHerdSolution.Core.Abstractions
{
    public interface IMessageBroker
    {
        Task PublishAsync<T>(string channel, T data);
        Task PublishAsync(string channel, string rawData);

    }
}
