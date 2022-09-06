using System;
using System.Reflection;
using Xunit;

namespace EventBus.Test
{
    public class EventBusTestBase
    {
        protected IEventBus TestEventBus;

        protected EventBusTestBase()
        {
            TestEventBus = new EventBus();
            TestEventBus.RegisterAllEventHandlerFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
