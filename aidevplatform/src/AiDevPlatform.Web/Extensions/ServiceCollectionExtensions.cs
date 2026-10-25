using AiDevPlatform.Web.Services;

namespace AiDevPlatform.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        // [来源: aspnet-core-aspnetcore-10.0#0247 p1644-1645]
        Action<HttpClient> configureClient = client =>
        {
            var address = new Uri(configuration["Api:BaseUrl"] ?? "http://localhost:54822/");
            if (!address.IsLoopback || address.Scheme != Uri.UriSchemeHttp ||
                !string.IsNullOrEmpty(address.UserInfo) || !string.IsNullOrEmpty(address.Query) ||
                !string.IsNullOrEmpty(address.Fragment))
                throw new InvalidOperationException("API 地址必须是无凭据的本机 HTTP 地址。");

            client.BaseAddress = address;
            client.Timeout = TimeSpan.FromSeconds(95);
            client.MaxResponseContentBufferSize = 262144;
            client.DefaultRequestHeaders.Add("X-LowCode-Client", "Web");
        };
        Func<HttpMessageHandler> createHandler = () => new HttpClientHandler
        {
            AllowAutoRedirect = false,
            UseProxy = false
        };
        services.AddHttpClient<PageDraftApiClient>(configureClient).ConfigurePrimaryHttpMessageHandler(createHandler);
        services.AddHttpClient<ApplicationsApiClient>(configureClient).ConfigurePrimaryHttpMessageHandler(createHandler);
        services.AddHttpClient<DevelopmentApiClient>(client =>
        {
            configureClient(client);
            client.Timeout = TimeSpan.FromSeconds(190);
            client.MaxResponseContentBufferSize = 4194304;
        }).ConfigurePrimaryHttpMessageHandler(createHandler);

        return services;
    }
}
