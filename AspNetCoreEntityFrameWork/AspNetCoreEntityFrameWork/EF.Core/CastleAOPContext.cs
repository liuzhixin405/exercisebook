using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EF.Core
{
    public class CastleAOPContext : IAOPContext
    {
        private readonly IInvocation _invocation;
        public CastleAOPContext(IInvocation invocation, IServiceProvider serviceProvider)
        {
            _invocation = invocation;
            ServiceProvider = serviceProvider;
        }
        public IServiceProvider ServiceProvider {get;}

        public object[] Arguments => _invocation.Arguments;

        public Type[] GenericAtguments => _invocation.GenericArguments;

        public MethodInfo Method => _invocation.Method;

        public MethodInfo MethodInvocationTarget => _invocation.MethodInvocationTarget;

        public object Proxy => _invocation.Proxy;

        public object ReturnValue { get => _invocation.ReturnValue; set => _invocation.ReturnValue = value; }

        public Type TargetType => _invocation.TargetType;

        public object InvocationTarget => _invocation.InvocationTarget;
    }
}
