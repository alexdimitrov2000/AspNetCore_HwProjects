using Eventures.Web.Extensions.CustomMiddlewares;
using Microsoft.AspNetCore.Builder;

namespace Eventures.Web.Extensions
{
    public static class SeedRolesMiddlewareExtensions
    {
        public static IApplicationBuilder SeedRoles(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SeedRolesMiddleware>();
        }
    }
}
