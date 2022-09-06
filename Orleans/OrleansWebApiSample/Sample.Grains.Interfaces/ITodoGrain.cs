using Orleans;
using Sample.Grains.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Grains.Interfaces
{
    public interface ITodoGrain:IGrainWithGuidKey
    {
        Task SetAsync(TodoItem item);
        Task ClearAsync();
        Task<TodoItem> GetAsync();
    }
}
