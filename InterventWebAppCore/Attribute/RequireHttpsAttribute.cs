using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InterventWebApp
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireHttpsAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool requiresHttps = false;
            bool.TryParse(AppSettings.GetAppSetting("AppSettings:RequiresHttps"), out requiresHttps);
            if (requiresHttps && context.HttpContext.Request.Scheme != Uri.UriSchemeHttps)
            {
                context.Result = new JsonResult(new { message = "HTTPS Required" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                if (!AuthorizationUtility.VerifySecretKey(context.HttpContext.Request, context.ActionDescriptor.RouteValues["controller"]!, requiresHttps))
                {
                    if (!AuthorizationUtility.VerifySecretKey(context.HttpContext.Request, context.ActionDescriptor.RouteValues["Action"]!, requiresHttps))
                    {
                        context.Result = new JsonResult(new { message = "Authorization header is not valid" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    }
                }
            }
        }
    }
}
