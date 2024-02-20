using System;
using System.Reflection;
using MyServiceAttribute;
using Panda.DynamicWebApi;

namespace Panda.DynamicWebApiSample.Dynamic
{
    internal class ServiceLocalSelectController : ISelectController
    {
        public bool IsController(Type type)
        {
            return type.IsPublic && type.GetCustomAttribute<MyAttribute>() != null;
        }
    }
}