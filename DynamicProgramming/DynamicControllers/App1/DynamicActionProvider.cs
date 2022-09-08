using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace App
{
public class DynamicActionProvider : IActionDescriptorProvider
{
    private readonly List<ControllerActionDescriptor> _actions;
    private readonly Func<string, IEnumerable<ControllerActionDescriptor>> _creator;

    public DynamicActionProvider(IServiceProvider serviceProvider, ICompiler compiler)
    {
        _actions = new List<ControllerActionDescriptor>();
        _creator = CreateActionDescrptors;

        IEnumerable<ControllerActionDescriptor> CreateActionDescrptors(string sourceCode)
        {
            var assembly = compiler.Compile(sourceCode, 
                Assembly.Load(new AssemblyName("System.Runtime")),
                typeof(object).Assembly,
                typeof(ControllerBase).Assembly,
                typeof(Controller).Assembly);
            var controllerTypes = assembly.GetTypes().Where(it => IsController(it));
            var applicationModel = CreateApplicationModel(controllerTypes);

            assembly = Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.Core"));
            var typeName = "Microsoft.AspNetCore.Mvc.ApplicationModels.ControllerActionDescriptorBuilder";
            var controllerBuilderType = assembly.GetTypes().Single(it => it.FullName == typeName);
            var buildMethod = controllerBuilderType.GetMethod("Build", BindingFlags.Static | BindingFlags.Public);
            return (IEnumerable<ControllerActionDescriptor>)buildMethod.Invoke(null, new object[] { applicationModel });
        }

        ApplicationModel CreateApplicationModel(IEnumerable<Type> controllerTypes)
        {
            var assembly = Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.Core"));
            var typeName = "Microsoft.AspNetCore.Mvc.ApplicationModels.ApplicationModelFactory";
            var factoryType = assembly.GetTypes().Single(it => it.FullName == typeName);
            var factory = serviceProvider.GetService(factoryType);
            var method = factoryType.GetMethod("CreateApplicationModel");
            var typeInfos = controllerTypes.Select(it => it.GetTypeInfo());
            return (ApplicationModel)method.Invoke(factory, new object[] { typeInfos });
        }

        bool IsController(Type typeInfo)
        {
            if (!typeInfo.IsClass) return false;
            if (typeInfo.IsAbstract) return false;
            if (!typeInfo.IsPublic) return false;
            if (typeInfo.ContainsGenericParameters) return false;
            if (typeInfo.IsDefined(typeof(NonControllerAttribute))) return false;
            if (!typeInfo.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) && !typeInfo.IsDefined(typeof(ControllerAttribute))) return false;
            return true;
        }
    }

    public int Order => -100;
    public void OnProvidersExecuted(ActionDescriptorProviderContext context) { }
    public void OnProvidersExecuting(ActionDescriptorProviderContext context)
    {
        foreach (var action in _actions)
        {
            context.Results.Add(action);
        }
    }
    public void AddControllers(string sourceCode) => _actions.AddRange(_creator(sourceCode));
}
}
