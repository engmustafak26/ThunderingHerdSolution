using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThunderingHerdSolution.Core.Abstractions
{
    public interface ICache
    {

        Task SetAsync<T>(string key, T data,
                                         TimeSpan? absoluteExpireTime = null,
                                         TimeSpan? unUsedExpireTime = null);

        Task SetAsync(string key, string jsonData,
                                         TimeSpan? absoluteExpireTime = null,
                                         TimeSpan? unUsedExpireTime = null);

        Task<T> GetAsync<T>(string key);

    }
}
