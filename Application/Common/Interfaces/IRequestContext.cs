namespace EasyMart.API.Application.Common.Interfaces
{
    public interface IRequestContext
    {
        string RequestId { get; }
        string ClientIp { get; }
    }
}
