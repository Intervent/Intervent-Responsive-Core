using System.Security.Claims;
using System.Security.Principal;

namespace InterventWebApp
{
    public static class GenericPrincipalExtensions
    {
        public static string FullName(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                return claimsIdentity.Claims.First(c => c.Type == "FullName").Value;
            }
            else
                return "";
        }

        public static string UserId(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                return claimsIdentity.Claims.First(c => c.Type == "UserId").Value;
            }
            else
                return "";
        }

        public static string DeviceId(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity.Claims.Any(c => c.Type == "DeviceId"))
                    return claimsIdentity.Claims.First(c => c.Type == "DeviceId").Value;
            }
            return "";
        }

        public static string SingleSignOn(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity.Claims.Any(c => c.Type == "SingleSignOn"))
                    return claimsIdentity.Claims.First(c => c.Type == "SingleSignOn").Value;
            }
            return "false";
        }

        public static string MobileSignOn(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity.Claims.Any(c => c.Type == "MobileSignOn"))
                    return claimsIdentity.Claims.First(c => c.Type == "MobileSignOn").Value;
            }
            return "false";
        }

        public static string[] ModuleList(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                var claim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "Module");
                if (claim != null && !string.IsNullOrEmpty(claim.Value))
                    return claim.Value.Split(',');
                return null;
            }
            else
                return null;
        }

        public static string RoleCode(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                var claim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "RoleCode");
                if (claim != null && !string.IsNullOrEmpty(claim.Value))
                    return claim.Value;
                return null;
            }
            else
                return null;
        }

        public static string TimeZone(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity.Claims.FirstOrDefault(c => c.Type == "TimeZone") != null)
                    return claimsIdentity.Claims.First(c => c.Type == "TimeZone").Value;
                else
                    return "";
            }
            else
                return "";
        }

        public static string ExpirationUrl(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity.Claims.FirstOrDefault(c => c.Type == "ExpirationUrl") != null)
                    return claimsIdentity.Claims.First(c => c.Type == "ExpirationUrl").Value;
                else
                    return "";
            }
            else
                return "";
        }
    }
}