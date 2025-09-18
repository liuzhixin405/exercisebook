using Microsoft.Extensions.Configuration;

namespace Framework.Core.Abstractions.Configuration;

/// <summary>
/// 配置构建器接口 - 建造者模式
/// 提供构建配置的抽象
/// </summary>
public interface IConfigurationBuilder
{
    /// <summary>
    /// 添加配置源
    /// </summary>
    /// <param name="source">配置源</param>
    /// <returns>配置构建器</returns>
    IConfigurationBuilder AddSource(IConfigurationSource source);

    /// <summary>
    /// 添加JSON配置
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="optional">是否可选</param>
    /// <param name="reloadOnChange">是否在文件变化时重新加载</param>
    /// <returns>配置构建器</returns>
    IConfigurationBuilder AddJsonFile(string path, bool optional = false, bool reloadOnChange = false);

    /// <summary>
    /// 添加环境变量
    /// </summary>
    /// <param name="prefix">前缀</param>
    /// <returns>配置构建器</returns>
    IConfigurationBuilder AddEnvironmentVariables(string? prefix = null);

    /// <summary>
    /// 添加命令行参数
    /// </summary>
    /// <param name="args">命令行参数</param>
    /// <returns>配置构建器</returns>
    IConfigurationBuilder AddCommandLine(string[] args);

    /// <summary>
    /// 添加内存配置
    /// </summary>
    /// <param name="initialData">初始数据</param>
    /// <returns>配置构建器</returns>
    IConfigurationBuilder AddInMemoryCollection(IEnumerable<KeyValuePair<string, string?>>? initialData = null);

    /// <summary>
    /// 添加配置适配器
    /// </summary>
    /// <param name="adapter">配置适配器</param>
    /// <returns>配置构建器</returns>
    IConfigurationBuilder AddAdapter(IConfigurationAdapter adapter);

    /// <summary>
    /// 构建配置
    /// </summary>
    /// <returns>配置根</returns>
    IConfigurationRoot Build();
}
