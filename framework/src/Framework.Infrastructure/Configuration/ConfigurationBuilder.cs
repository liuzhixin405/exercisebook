using Framework.Core.Abstractions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.Memory;

namespace Framework.Infrastructure.Configuration;

/// <summary>
/// 配置构建器实现 - 建造者模式
/// 提供构建配置的实现
/// </summary>
public class ConfigurationBuilder : Framework.Core.Abstractions.Configuration.IConfigurationBuilder
{
    private readonly Microsoft.Extensions.Configuration.ConfigurationBuilder _builder;
    private readonly List<IConfigurationAdapter> _adapters;

    /// <summary>
    /// 构造函数
    /// </summary>
    public ConfigurationBuilder()
    {
        _builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
        _adapters = new List<IConfigurationAdapter>();
    }

    /// <inheritdoc />
    public Framework.Core.Abstractions.Configuration.IConfigurationBuilder AddSource(Microsoft.Extensions.Configuration.IConfigurationSource source)
    {
        _builder.Add(source);
        return this;
    }

    /// <inheritdoc />
    public Framework.Core.Abstractions.Configuration.IConfigurationBuilder AddJsonFile(string path, bool optional = false, bool reloadOnChange = false)
    {
        _builder.AddJsonFile(path, optional, reloadOnChange);
        return this;
    }

    /// <inheritdoc />
    public Framework.Core.Abstractions.Configuration.IConfigurationBuilder AddEnvironmentVariables(string? prefix = null)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            _builder.AddEnvironmentVariables();
        }
        else
        {
            _builder.AddEnvironmentVariables(prefix);
        }
        return this;
    }

    /// <inheritdoc />
    public Framework.Core.Abstractions.Configuration.IConfigurationBuilder AddCommandLine(string[] args)
    {
        _builder.AddCommandLine(args);
        return this;
    }

    /// <inheritdoc />
    public Framework.Core.Abstractions.Configuration.IConfigurationBuilder AddInMemoryCollection(IEnumerable<KeyValuePair<string, string?>>? initialData = null)
    {
        _builder.AddInMemoryCollection(initialData);
        return this;
    }

    /// <inheritdoc />
    public Framework.Core.Abstractions.Configuration.IConfigurationBuilder AddAdapter(IConfigurationAdapter adapter)
    {
        if (adapter == null)
            throw new ArgumentNullException(nameof(adapter));

        _adapters.Add(adapter);
        return this;
    }

    /// <inheritdoc />
    public Microsoft.Extensions.Configuration.IConfiguration Build()
    {
        Microsoft.Extensions.Configuration.IConfiguration configuration = _builder.Build();

        // 应用所有适配器
        foreach (var adapter in _adapters)
        {
            configuration = adapter.Adapt(configuration);
        }

        return configuration;
    }
}
