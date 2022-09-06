using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Sample.Grains.Interfaces;
using Sample.Grains.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Grains
{
    public class TodoGrain : Grain, ITodoGrain
    {
        private readonly ILogger<TodoGrain> logger;
        private readonly IPersistentState<State> state;

        private string GrainType=>nameof(TodoGrain);
        private Guid GrainKey=>this.GetPrimaryKey();
        public TodoGrain(ILogger<TodoGrain> logger,[PersistentState("State")] IPersistentState<State> state)
        {
            this.logger = logger;
            this.state = state;
        }
        public async Task ClearAsync()
        {
            if (state.State.Item == null) return;
            var itemKey = state.State.Item.Key;
            var ownerKey = state.State.Item.OwnerKey;

            await GrainFactory.GetGrain<ITodoManagerGrain>(ownerKey).UnregisterAsync(itemKey);
            await state.ClearStateAsync();
            logger.LogInformation(
               "{@GrainType} {@GrainKey} is now cleared",
               GrainType, GrainKey);
            GetStreamProvider("SMS").GetStream<TodoNotification>(ownerKey, nameof(ITodoGrain)).OnNextAsync(new TodoNotification(itemKey,null)).Ignore();
            DeactivateOnIdle();
        }

        public Task<TodoItem> GetAsync()
        {
            return Task.FromResult(state.State.Item);
        }

        public async Task SetAsync(TodoItem item)
        {
            if (item.Key != GrainKey)
            {
                throw new InvalidOperationException();
            }
            state.State.Item = item;
            await state.WriteStateAsync();
            await GrainFactory.GetGrain<ITodoManagerGrain>(item.OwnerKey).RegisterAsync(item.Key);
            logger.LogInformation("{@GrainType} {@GrainKey} now contains {@Todo}",
                GrainType, GrainKey, item);
            GetStreamProvider("SMS").GetStream<TodoNotification>(item.OwnerKey,nameof(ITodoGrain)).OnNextAsync(new TodoNotification(item.Key,item)).Ignore();
        }
        public class State
        {
            public TodoItem Item { get; set; }
        }
    }
}
