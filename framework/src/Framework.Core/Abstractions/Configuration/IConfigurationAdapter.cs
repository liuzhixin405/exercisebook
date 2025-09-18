using Microsoft.Extensions.Configuration;

namespace Framework.Core.Abstractions.Configuration;

/// <summary>
/// 配置适配器接口 - 适配器模式
/// 提供不同配置源之间的适配
/// </summary>
public interface IConfigurationAdapter
{
    /// <summary>
    /// 适配器名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 适配配置
    /// </summary>
    /// <param name="configuration">原始配置</param>
    /// <returns>适配后的配置</returns>
    IConfiguration Adapt(IConfiguration configuration);

    /// <summary>
    /// 是否支持指定的配置源
    /// </summary>
    /// <param name="source">配置源</param>
    /// <returns>是否支持</returns>
    bool CanAdapt(IConfigurationSource source);
}
