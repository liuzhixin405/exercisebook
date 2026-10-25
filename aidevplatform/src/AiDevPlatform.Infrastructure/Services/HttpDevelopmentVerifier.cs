using System.Text;
using System.Text.Json;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.DTOs.Application;
using Microsoft.Extensions.Configuration;

namespace AiDevPlatform.Infrastructure.Services;

public sealed class HttpDevelopmentVerifier : IDevelopmentVerifier
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
    private readonly IHttpClientFactory _clients;
    private readonly IConfiguration _configuration;

    public HttpDevelopmentVerifier(IHttpClientFactory clients, IConfiguration configuration)
    {
        _clients = clients;
        _configuration = configuration;
    }

    public async Task<VerificationReport> VerifyAsync(VerificationRequest request, CancellationToken cancellationToken)
    {
        if (!bool.TryParse(_configuration["Verification:Enabled"], out var enabled) || !enabled ||
            !Uri.TryCreate(_configuration["Verification:Endpoint"], UriKind.Absolute, out var endpoint) ||
            endpoint.Scheme != Uri.UriSchemeHttp || !endpoint.IsLoopback || endpoint.UserInfo.Length != 0 ||
            endpoint.Query.Length != 0 || endpoint.Fragment.Length != 0)
            throw new NotSupportedException("请配置可信的本机隔离验证器。");
        using var content = new StringContent(JsonSerializer.Serialize(request, Options), Encoding.UTF8, "application/json");
        using var response = await _clients.CreateClient("Development-Verification").PostAsync(endpoint, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<VerificationReport>(await response.Content.ReadAsStringAsync(cancellationToken), Options)
            ?? throw new JsonException("验证报告为空。");
    }
}