using Framework.Core.Abstractions.Configuration;
using Microsoft.Extensions.Configuration;
using FrameworkConfigurationBuilder = Framework.Core.Abstractions.Configuration.IConfigurationBuilder;
using MicrosoftConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace Framework.Infrastructure.Configuration;

/// <summary>
/// 配置构建器实现 - 建造者模式
/// 提供构建配置的实现
/// </summary>
public class ConfigurationBuilder : FrameworkConfigurationBuilder
{
    private readonly MicrosoftConfigurationBuilder _builder;
    private readonly List<IConfigurationAdapter> _adapters;

    /// <summary>
    /// 构造函数
    /// </summary>
    public ConfigurationBuilder()
    {
        _builder = new MicrosoftConfigurationBuilder();
        _adapters = new List<IConfigurationAdapter>();
    }

    /// <inheritdoc />
    public FrameworkConfigurationBuilder AddSource(IConfigurationSource source)
    {
        _builder.Add(source);
        return this;
    }

    /// <inheritdoc />
    public FrameworkConfigurationBuilder AddJsonFile(string path, bool optional = false, bool reloadOnChange = false)
    {
        _builder.AddJsonFile(path, optional, reloadOnChange);
        return this;
    }

    /// <inheritdoc />
    public FrameworkConfigurationBuilder AddEnvironmentVariables(string? prefix = null)
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
    public FrameworkConfigurationBuilder AddCommandLine(string[] args)
    {
        _builder.AddCommandLine(args);
        return this;
    }

    /// <inheritdoc />
    public FrameworkConfigurationBuilder AddInMemoryCollection(IEnumerable<KeyValuePair<string, string?>>? initialData = null)
    {
        _builder.AddInMemoryCollection(initialData);
        return this;
    }

    /// <inheritdoc />
    public FrameworkConfigurationBuilder AddAdapter(IConfigurationAdapter adapter)
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
            var adaptedConfig = adapter.Adapt(configuration);
            if (adaptedConfig is IConfigurationRoot adaptedRoot)
            {
                configuration = adaptedRoot;
            }
        }

        return configuration;
    }
}
