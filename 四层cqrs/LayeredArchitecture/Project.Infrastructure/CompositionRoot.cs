using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure
{
    public static class CompositionRoot
    {
        private static IContainer _container;
        public static void SetContainer(IContainer container)=>_container=container;
        internal static ILifetimeScope BeginLifetimeScope()=>_container.BeginLifetimeScope();
    }
}
