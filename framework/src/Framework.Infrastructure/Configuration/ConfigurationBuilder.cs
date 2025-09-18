using Framework.Core.Abstractions.Configuration;
using Microsoft.Extensions.Configuration;

namespace Framework.Infrastructure.Configuration;

/// <summary>
/// 配置构建器实现 - 建造者模式
/// 提供构建配置的实现
/// </summary>
public class ConfigurationBuilder : IConfigurationBuilder
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
    public IConfigurationBuilder AddSource(IConfigurationSource source)
    {
        _builder.Add(source);
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder AddJsonFile(string path, bool optional = false, bool reloadOnChange = false)
    {
        _builder.AddJsonFile(path, optional, reloadOnChange);
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder AddEnvironmentVariables(string? prefix = null)
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
    public IConfigurationBuilder AddCommandLine(string[] args)
    {
        _builder.AddCommandLine(args);
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder AddInMemoryCollection(IEnumerable<KeyValuePair<string, string?>>? initialData = null)
    {
        _builder.AddInMemoryCollection(initialData);
        return this;
    }

    /// <inheritdoc />
    public IConfigurationBuilder AddAdapter(IConfigurationAdapter adapter)
    {
        if (adapter == null)
            throw new ArgumentNullException(nameof(adapter));

        _adapters.Add(adapter);
        return this;
    }

    /// <inheritdoc />
    public IConfigurationRoot Build()
    {
        var configuration = _builder.Build();

        // 应用所有适配器
        foreach (var adapter in _adapters)
        {
            configuration = adapter.Adapt(configuration);
        }

        return configuration;
    }
}
