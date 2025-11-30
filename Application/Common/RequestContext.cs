using EasyMart.API.Application.Common.Interfaces;

namespace EasyMart.API.Application.Common
{
    public sealed class RequestContext : IRequestContext
    {
        public string RequestId { get; internal set; } = string.Empty;
        public string ClientIp { get; internal set; } = string.Empty;
    }
}
