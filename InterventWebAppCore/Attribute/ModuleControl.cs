using Microsoft.AspNetCore.Authorization;

namespace InterventWebApp
{
    public class ModuleControlAttribute : AuthorizeAttribute
    {
        public string ModuleName { get; set; }
        public string[] RoleList { get; set; }

        public ModuleControlAttribute(string module)
            : base()
        {
            ModuleName = module;
            RoleList = null;
        }

        public ModuleControlAttribute(string module, params string[] roleCode)
            : base()
        {
            RoleList = roleCode;
            ModuleName = module;
        }

        /*public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "Index",
                    controller = "Home"
                }));
            }
            else if (!string.IsNullOrEmpty(ModuleName) && (filterContext.HttpContext.User.ModuleList() == null || !filterContext.HttpContext.User.ModuleList().Contains(ModuleName)))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "NotAuthorized",
                    controller = "Account"
                }));
            }
            else if (RoleList != null && RoleList.Count() > 0 && (string.IsNullOrEmpty(filterContext.HttpContext.User.RoleCode()) || !CommonUtility.RoleContains(RoleList, filterContext.HttpContext.User.RoleCode())))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "NotAuthorized",
                    controller = "Account"
                }));
            }
            base.OnAuthorization(filterContext);
        }*/

        /*protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "Index",
                    controller = "Home"
                }));
            }
            else if (!string.IsNullOrEmpty(ModuleName) && (filterContext.HttpContext.User.ModuleList() == null || !filterContext.HttpContext.User.ModuleList().Contains(ModuleName)))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "NotAuthorized",
                    controller = "Account"
                }));
            }
            else if (RoleList != null && RoleList.Count() > 0 && (string.IsNullOrEmpty(filterContext.HttpContext.User.RoleCode()) || !CommonUtility.RoleContains(RoleList, filterContext.HttpContext.User.RoleCode())))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "NotAuthorized",
                    controller = "Account"
                }));
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }*/
    }
}