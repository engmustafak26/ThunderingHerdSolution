using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThunderingHerdSolution.Core.Abstractions
{
    public interface IJobRescheduler
    {
        Task<DateTime> InvokeEventHandlerAsync(IEventHandler eventHandler, bool publishToOwnChannel = false);

    }
}
