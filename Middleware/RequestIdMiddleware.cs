using EasyMart.API.Application.Common;
using System;

namespace EasyMart.API.Middleware
{
    public sealed class RequestIdMiddleware
    {
        private const string HeaderName = "X-Request-Id";

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestIdMiddleware> _logger;

        public RequestIdMiddleware(
            RequestDelegate next,
            ILogger<RequestIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext httpContext,
            RequestContext requestContext)
        {
            
            var requestId = CreateRequestId();
            var clientIp = ResolveClientIp(httpContext);


            requestContext.RequestId = requestId;
            requestContext.ClientIp = clientIp;

            // Expose to client (optional)
            httpContext.Response.Headers[HeaderName] = requestId;

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["RequestId"] = requestId,
                ["ClientIp"] = clientIp
            }))
            {
                await _next(httpContext);
            }
        }

        private static string CreateRequestId()
        {
            // Compact, high-entropy, sortable-friendly
            return Guid.NewGuid().ToString("N");
        }
        private static string ResolveClientIp(HttpContext context)
        {
            // When behind reverse proxies (NGINX, Azure, etc.)
            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwarded))
            {
                return forwarded.Split(',')[0].Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
