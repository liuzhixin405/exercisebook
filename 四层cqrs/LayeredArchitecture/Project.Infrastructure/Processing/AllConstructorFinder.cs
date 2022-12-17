﻿using Autofac.Core.Activators.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing
{
    public class AllConstructorFinder : IConstructorFinder
    {
        private
            static readonly ConcurrentDictionary<Type, ConstructorInfo[]> Cache = new ConcurrentDictionary<Type, ConstructorInfo[]>();
        public ConstructorInfo[] FindConstructors(Type targetType)
        {
            var result = Cache.GetOrAdd(targetType, t => t.GetTypeInfo().DeclaredConstructors.ToArray());
            return result.Length > 0 ? result : throw new NoConstructorsFoundException(targetType);
        }
    }
}
