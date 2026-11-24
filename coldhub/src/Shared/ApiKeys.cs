using System.Security.Cryptography;
using System.Text;

namespace ColdHub.Shared;

/// <summary>
/// API key 只存哈希。Admin 在创建 key 时把明文返回一次，之后只保存 SHA-256；Hub 向 Admin 取回哈希集合，
/// 对客户端带来的 key 做同样的哈希再比较。
/// </summary>
public static class ApiKeys
{
    public const string Scheme = "ApiKey";

    public static string Hash(string key) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key)));

    public static string NewKey() => "ch_" + Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();

    /// <summary>从 <c>Authorization: ApiKey xxx</c> 取出 key；格式不对返回 null（Bearer 也算不对）。</summary>
    public static string? FromHeader(string? authorization)
    {
        const string prefix = Scheme + " ";
        return authorization is not null && authorization.StartsWith(prefix, StringComparison.Ordinal)
            ? authorization[prefix.Length..].Trim()
            : null;
    }
}
