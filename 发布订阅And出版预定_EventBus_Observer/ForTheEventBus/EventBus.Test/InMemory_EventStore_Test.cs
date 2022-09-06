using EventBus.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EventBus.Test
{
    public class InMemory_EventStore_Test
    {
        [Fact]
        public void After_Creation_Should_Be_Empty()
        {
            var eventStore = new InMemoryEventStore();
            eventStore.IsEmpty.Equals(true);
        }

        [Fact]
        public void After_Register_Should_Contain_The_Event()
        {
            var eventStore = new InMemoryEventStore();
            eventStore.AddRegister(typeof(TestEventData), typeof(TestEventHandler));
            eventStore.HasRegisterForEvent(typeof(TestEventData)).Equals(true);
        }

        [Fact]

        public void After_UnRegister_Event_Should_No_Longger_Exist()
        {
            var eventStore = new InMemoryEventStore();
            eventStore.AddRegister(typeof(TestEventData), typeof(TestEventHandler));
            eventStore.RemoveRegister(typeof(TestEventData), typeof(TestEventHandler));
            eventStore.HasRegisterForEvent(typeof(TestEventData)).Equals(false);
        }

        [Fact]
        public void After_Clear_Should_Be_Empty()
        {
            var eventStore = new InMemoryEventStore();
            eventStore.AddRegister(typeof(TestEventData), typeof(TestEventHandler));
            eventStore.IsEmpty.Equals(false);

            eventStore.Clear();

            eventStore.IsEmpty.Equals(true);
        }
    }
}
