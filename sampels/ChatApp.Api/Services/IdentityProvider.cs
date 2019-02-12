using AspNet.Security.OpenIdConnect.Primitives;
using ChatApp.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Api
{
    public class IdentityProvider : IIdentityProvider<string>
    {
        public IdentityProvider(IHttpContextAccessor accessor)
        {
            var httpContext = accessor.HttpContext;
            CurrentUserId = httpContext?.User.FindFirst(OpenIdConnectConstants.Claims.Subject)?.Value?.Trim();
        }
        public string CurrentUserId { get; }
    }
}