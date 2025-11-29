using EasyMart.API.Application.Common;

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

            requestContext.RequestId = requestId;

            // Expose to client (optional)
            httpContext.Response.Headers[HeaderName] = requestId;

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["RequestId"] = requestId
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
    }
}
