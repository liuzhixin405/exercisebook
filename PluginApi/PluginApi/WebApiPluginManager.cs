using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using PluginApi;
using IPluginLibrary;

public class WebApiPluginManager
{
    private List<IWebApiPlugin> plugins;

    public WebApiPluginManager()
    {
        plugins = new List<IWebApiPlugin>();
    }

    public void LoadPlugins(string pluginsDirectory)
    {
        var pluginFiles = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(),"pluginFolder"), "*.dll");

        foreach (var file in pluginFiles)
        {
            var assembly = Assembly.LoadFrom(file);
            var types = assembly.GetTypes().Where(x=>x.Name.Equals("Plugin")).ToList();

            foreach (var type in types)
            {
                if (typeof(IWebApiPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    var plugin = (IWebApiPlugin)Activator.CreateInstance(type);
                    plugins.Add(plugin);
                }
            }
        }
    }

    public void ConfigureServices(IServiceCollection services)
    {
        foreach (var plugin in plugins)
        {
            plugin.ConfigureServices(services);
        }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        foreach (var plugin in plugins)
        {
            plugin.Configure(app, env);
        }
    }
}
