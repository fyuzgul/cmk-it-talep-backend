using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CMKITTalep.API.Attributes
{
    public class RequireAdminAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // JWT authentication'ı manuel olarak kontrol et
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            
            string token = null;
            
            if (authHeader != null)
            {
                if (authHeader.StartsWith("Bearer "))
                {
                    // Bearer prefix'i varsa token'ı al
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }
                else
                {
                    // Bearer prefix'i yoksa direkt token olarak kabul et
                    token = authHeader.Trim();
                }
                
                // Token'ı validate et
                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    var jwtSettings = context.HttpContext.RequestServices.GetRequiredService<CMKITTalep.API.Models.JwtSettings>();
                    var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
                    
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);
                    
                    var jwtToken = (JwtSecurityToken)validatedToken;
                    
                    // UserType claim'ini kontrol et
                    var userTypeClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "UserType")?.Value;
                    
                    if (userTypeClaim?.ToLower() != "admin")
                    {
                        context.Result = new ForbidResult();
                        return Task.CompletedTask;
                    }
                }
                catch
                {
                    context.Result = new UnauthorizedResult();
                    return Task.CompletedTask;
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
                return Task.CompletedTask;
            }
            
            return Task.CompletedTask;
        }
    }
}
