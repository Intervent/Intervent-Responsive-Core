using Intervent.DAL;
using Intervent.Web.DataLayer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System.Security.Claims;

namespace InterventWebApp.Controllers
{
    public class MobileOAuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public MobileOAuthController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        public async Task<ActionResult> AuthenticateUser()
        {
            try
            {
                if (await AuthenticateUserOAuth())
                {
                    return RedirectToAction("Stream", "Participant", new { ForceParticipant = true });
                }
            }
            catch { }
            return RedirectToAction("NotAuthorized", "Account");
        }

        private async Task<bool> AuthenticateUserOAuth()
        {
            try
            {
                var userIdentity = (ClaimsIdentity)User.Identity;
                if (userIdentity == null)
                    return false;

                string userId = userIdentity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).FirstOrDefault();
                string deviceId = userIdentity.Claims.Where(c => c.Type == "DeviceId").Select(c => c.Value).FirstOrDefault();

                if (userIdentity.AuthenticationType != CookieAuthenticationDefaults.AuthenticationScheme)
                    return false;
                else
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.Session.Clear();
                    //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    var response = await AccountUtility.GetUser(_userManager, null, null, int.Parse(userId), null, null);
                    if (response != null && response.User != null)
                    {
                        var userDto = response.User;
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        var identity = new ClaimsIdentity("InterventMobileOauthLogin");
                        HttpContext.Session.SetInt32(SessionContext.UserId, userDto.Id);
                        HttpContext.Session.SetInt32(SessionContext.ParticipantId, userDto.Id);
                        HttpContext.Session.SetString(SessionContext.TermsSSO, userDto.Organization.TermsForSSO.ToString());
                        HttpContext.Session.SetString(SessionContext.TermsAccepted, userDto.TermsAccepted.ToString());
                        identity.AddClaim(new Claim("UserId", userDto.Id.ToString()));
                        identity.AddClaim(new Claim("FullName", userDto.FirstName + " " + userDto.LastName));
                        identity.AddClaim(new Claim("Module", AccountController.GetGroups(userDto)));
                        identity.AddClaim(new Claim("RoleCode", AccountController.GetRoleCodes(userDto)));
                        identity.AddClaim(new Claim("ExpirationUrl", userDto.Organization.Url));
                        identity.AddClaim(new Claim("DeviceId", deviceId));
                        identity.AddClaim(new Claim("SingleSignOn", "false"));
                        identity.AddClaim(new Claim("MobileSignOn", "true"));
                        if (userDto.TimeZoneId.HasValue)
                            identity.AddClaim(new Claim("TimeZone", CommonUtility.GetTimeZones(userDto.TimeZoneId).TimeZones[0].TimeZoneId));
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "AuthenticateUserOAuth", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return false;
            }
            return false;
        }

        private void LogRequestResponse(string message)
        {
            LogReader logReader = new LogReader();
            var logEvent = new LogEventInfo(NLog.LogLevel.Info, "AuthenticateUserOAuth", null, message, null, null);
            logReader.WriteLogMessage(logEvent);
        }
    }
}
