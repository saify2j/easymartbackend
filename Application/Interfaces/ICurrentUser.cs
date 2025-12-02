namespace EasyMart.API.Application.Interfaces
{
    public interface ICurrentUser
    {
        int UserId { get; }
        string? Username { get; }
        IReadOnlyList<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}
