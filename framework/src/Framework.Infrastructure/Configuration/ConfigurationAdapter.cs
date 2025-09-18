using Framework.Core.Abstractions.Configuration;
using Microsoft.Extensions.Configuration;

namespace Framework.Infrastructure.Configuration;

/// <summary>
/// 配置适配器实现 - 适配器模式
/// 提供不同配置源之间的适配
/// </summary>
public class ConfigurationAdapter : IConfigurationAdapter
{
    private readonly Func<IConfiguration, IConfiguration> _adaptFunc;
    private readonly Func<IConfigurationSource, bool> _canAdaptFunc;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name">适配器名称</param>
    /// <param name="adaptFunc">适配函数</param>
    /// <param name="canAdaptFunc">是否支持适配的函数</param>
    public ConfigurationAdapter(
        string name,
        Func<IConfiguration, IConfiguration> adaptFunc,
        Func<IConfigurationSource, bool> canAdaptFunc)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _adaptFunc = adaptFunc ?? throw new ArgumentNullException(nameof(adaptFunc));
        _canAdaptFunc = canAdaptFunc ?? throw new ArgumentNullException(nameof(canAdaptFunc));
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public IConfiguration Adapt(IConfiguration configuration)
    {
        return _adaptFunc(configuration);
    }

    /// <inheritdoc />
    public bool CanAdapt(IConfigurationSource source)
    {
        return _canAdaptFunc(source);
    }
}

/// <summary>
/// 环境变量配置适配器
/// </summary>
public class EnvironmentConfigurationAdapter : IConfigurationAdapter
{
    /// <inheritdoc />
    public string Name => "Environment";

    /// <inheritdoc />
    public IConfiguration Adapt(IConfiguration configuration)
    {
        var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
        builder.AddConfiguration(configuration);
        builder.AddEnvironmentVariables();
        return builder.Build();
    }

    /// <inheritdoc />
    public bool CanAdapt(IConfigurationSource source)
    {
        return source is Microsoft.Extensions.Configuration.EnvironmentVariables.EnvironmentVariablesConfigurationSource;
    }
}

/// <summary>
/// JSON配置适配器
/// </summary>
public class JsonConfigurationAdapter : IConfigurationAdapter
{
    private readonly string _path;
    private readonly bool _optional;
    private readonly bool _reloadOnChange;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="path">JSON文件路径</param>
    /// <param name="optional">是否可选</param>
    /// <param name="reloadOnChange">是否在文件变化时重新加载</param>
    public JsonConfigurationAdapter(string path, bool optional = false, bool reloadOnChange = false)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
        _optional = optional;
        _reloadOnChange = reloadOnChange;
    }

    /// <inheritdoc />
    public string Name => "JSON";

    /// <inheritdoc />
    public IConfiguration Adapt(IConfiguration configuration)
    {
        var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
        builder.AddConfiguration(configuration);
        builder.AddJsonFile(_path, _optional, _reloadOnChange);
        return builder.Build();
    }

    /// <inheritdoc />
    public bool CanAdapt(IConfigurationSource source)
    {
        return source is Microsoft.Extensions.Configuration.Json.JsonConfigurationSource;
    }
}
