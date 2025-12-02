using EasyMart.API.Application.Interfaces;
using System.Security.Claims;

namespace EasyMart.API.Application.Services
{
  

    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User =>
            _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;

        public int UserId =>
            int.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
                ? id : 0;

        public string? Username =>
            User?.FindFirstValue(ClaimTypes.Name);

        public IReadOnlyList<string> Roles =>
            User?
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList()
            ?? [];
    }
}
