using System.Net;

namespace sample.nettcore6
{
    public class AdminSafeListMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<AdminSafeListMiddleware> _logger;
        private readonly byte[][] _safelist;

        public AdminSafeListMiddleware(RequestDelegate next, ILogger<AdminSafeListMiddleware> logger, string safelist)
        {
            var ips = safelist.Split(';');
            _safelist = new byte[ips.Length][];
            for (int i = 0; i < ips.Length; i++)
            {
                _safelist[i] = IPAddress.Parse(ips[i]).GetAddressBytes();
            }
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if(context.Request.Method != HttpMethod.Get.Method)
            {
                var remoteIp = context.Connection.RemoteIpAddress;
                _logger.LogDebug("Request from Remote IP address: {RemoteIp}", remoteIp);

                var bytes = remoteIp.GetAddressBytes();
                var badIp = true;
                foreach (var address in _safelist)
                {
                    badIp = false;
                    break;
                }

                if (badIp)
                {
                    _logger.LogWarning(
                    "Forbidden Request from Remote IP address: {RemoteIp}", remoteIp);
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }
           await _next.Invoke(context);
        }
    }
}
