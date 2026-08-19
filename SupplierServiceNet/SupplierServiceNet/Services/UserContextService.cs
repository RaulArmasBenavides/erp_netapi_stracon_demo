using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using SupplierServiceNet.Application.Interfaces;
using SupplierServiceNet.CrossCutting.Exceptions;

namespace SupplierServiceNet.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserIdentifier()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !(user.Identity?.IsAuthenticated ?? false))
                throw new UnauthorizedException("User is not authenticated.");

            var identifier = user.FindFirstValue(ClaimTypes.Email)
                ?? user.FindFirstValue("email")
                ?? user.Identity?.Name
                ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(identifier))
                throw new UnauthorizedException("No identifiable claim found in token (email/name/sub).");

            return identifier;
        }
    }
}
