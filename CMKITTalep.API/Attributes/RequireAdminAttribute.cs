using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CMKITTalep.API.Attributes
{
    public class RequireAdminAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userTypeClaim = user.FindFirst("UserType")?.Value;
            
            if (userTypeClaim != "Admin")
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
