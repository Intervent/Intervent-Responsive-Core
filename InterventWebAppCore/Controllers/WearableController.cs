using Intervent.Business;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using System.IdentityModel.Tokens.Jwt;

namespace InterventWebApp.Controllers
{
    public class WearableController : Controller
    {
        private readonly AppSettings _appSettings;

        public WearableController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [Authorize]
        public ActionResult Wearable()
        {
            return View("Index");
        }

        [Authorize]
        public JsonResult ListWearableDevices()
        {
            var deviceTypes = WearableUtility.GetWearableDevices((int)WearableDeviceType.Web);
            var connectedDevices = WearableUtility.GetUserWearableDevices(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            return Json(new
            {
                Result = "OK",
                wearableDevices = deviceTypes.Select(x => new
                {
                    deviceId = x.Id,
                    Icon = x.Icon,
                    Name = x.Name,
                    AuthUrl = x.AuthUrl,
                    connectedDeviceId = connectedDevices.Where(y => y.WearableDeviceId == x.Id).FirstOrDefault() != null ? connectedDevices.Where(y => y.WearableDeviceId == x.Id).FirstOrDefault().Id.ToString() : ""
                }),
                connectedDeviceCount = connectedDevices.Where(x => x.WearableDevice.Type == (int)WearableDeviceType.Web).Count(),
            });
        }

        [Authorize]
        public JsonResult DisconnectDevice(int deviceId)
        {
            var deviceTypes = WearableUtility.GetWearableDevices((int)WearableDeviceType.Web);
            var device = WearableUtility.GetUserWearableDevicesById(deviceId);
            bool response = false;
            string message = "You have successfully disconnected.";
            var request = HttpContext.Request;
            var baseURL = $"{request.Scheme}://{request.Host}";
            if (deviceTypes.Where(x => x.Id == device.WearableDeviceId && x.Name.ToLower() == "fitbit").Count() >= 0)
            {
                bool status = Task.Run(() => new FitbitClient().RevokeAccessAsync(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, device.RefreshToken, device.ExternalUserId, device.WearableDeviceId, _appSettings.FitbitApiUrl, _appSettings.FitbitClientId, _appSettings.FitbitClientSecret)).Result;
                if (!status)
                    message = "Unknown issue occurred: Please contact support.";
                else
                    response = WearableUtility.AddOrEditWearableDevice(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, device.ExternalUserId, null, null, device.WearableDeviceId, false, null);
            }
            else if (deviceTypes.Where(x => x.Id == device.WearableDeviceId && x.Name.ToLower() == "dexcom").Count() >= 0)
            {
                response = WearableUtility.AddOrEditWearableDevice(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, device.ExternalUserId, null, null, device.WearableDeviceId, false, null);
            }
            else if (deviceTypes.Where(x => x.Id == device.WearableDeviceId && x.Name.ToLower() == "garmin").Count() >= 0)
            {
                var deRegistrationResponse = Task.Run(() => new GarminClient().UserDeRegistration(device.Token, device.OauthTokenSecret, _appSettings.GarminUserEndPointUrl, _appSettings.GarminConsumerKey, _appSettings.GarminConsumerSecret)).Result;
                if (!deRegistrationResponse.Status)
                    message = "Unknown issue occurred: Please contact support.";
                else
                    response = WearableUtility.AddOrEditWearableDevice(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, device.ExternalUserId, null, null, device.WearableDeviceId, false, null);
            }
            else if (deviceTypes.Where(x => x.Id == device.WearableDeviceId && x.Name.ToLower() == "withings").Count() >= 0)
            {
                bool status = Task.Run(() => new WithingsClient(null).RevokeAccessAsync(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, device.RefreshToken, device.ExternalUserId, device.WearableDeviceId, _appSettings.WithingsApiUrl, _appSettings.WithingsClientId, _appSettings.WithingsClientSecret)).Result;
                if (!status)
                    message = "Unknown issue occurred: Please contact support.";
                else
                    response = WearableUtility.AddOrEditWearableDevice(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, device.ExternalUserId, null, null, device.WearableDeviceId, false, null);
            }
            else if (deviceTypes.Where(x => x.Id == device.WearableDeviceId && x.Name.ToLower() == "omron").Count() >= 0)
            {
                bool status = Task.Run(() => new OmronClient(baseURL + "/OAuth/OmronCallback").RevokeAccessAsync(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, device.RefreshToken, device.ExternalUserId, device.WearableDeviceId, _appSettings.OmronApiUrl, _appSettings.OmronClientId, _appSettings.OmronClientSecret)).Result;
                if (!status)
                    message = "Unknown issue occurred: Please contact support.";
                else
                    response = WearableUtility.AddOrEditWearableDevice(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, device.ExternalUserId, null, null, device.WearableDeviceId, false, null);
            }
            else if (deviceTypes.Where(x => x.Id == device.WearableDeviceId && x.Name.ToLower() == "google fit").Count() >= 0)
            {
                bool status = Task.Run(() => new GoogleFitClient(baseURL + "/OAuth/GoogleFitCallback").RevokeAccessAsync(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, device.RefreshToken, device.ExternalUserId, device.WearableDeviceId, _appSettings.GoogleFitClientId, _appSettings.GoogleFitClientSecret)).Result;
                if (!status)
                    message = "Unknown issue occurred: Please contact support.";
                else
                    response = WearableUtility.AddOrEditWearableDevice(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, device.ExternalUserId, null, null, device.WearableDeviceId, false, null);
            }
            return Json(new { Result = response, Message = message });
        }

        #region Garmin

        [Authorize]
        public ActionResult GarminAuth()
        {
            var oAuth = new GarminClient();
            var authDetails = Task.Run(() => oAuth.GenerateAuthUrl(_appSettings.GarminApiUrl, _appSettings.GarminConnectUrl, _appSettings.GarminConsumerKey, _appSettings.GarminConsumerSecret)).Result;
            if (authDetails != null)
            {
                HttpContext.Session.SetString(SessionContext.GarminOAuthSecretKey, authDetails.oauth_token_secret);
                return Redirect(authDetails.url);
            }
            return RedirectToAction("Wearable", "Wearable");
        }

        public ActionResult GarminCallback()
        {
            var authenticator = new GarminClient();
            string oauth_verifier = Request.Query["oauth_verifier"];
            string oauth_token = Request.Query["oauth_token"];
            GarminOAuth1 accessToken = Task.Run(() => authenticator.GetAccessTokenForAsync(oauth_token, oauth_verifier, HttpContext.Session.GetString(SessionContext.GarminOAuthSecretKey), _appSettings.GarminApiUrl, _appSettings.GarminUserEndPointUrl, _appSettings.GarminConsumerKey, _appSettings.GarminConsumerSecret)).Result;
            if (accessToken != null)
            {
                var deviceTypes = WearableUtility.GetWearableDevices((int)WearableDeviceType.Web);
                int userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                WearableUtility.AddOrEditWearableDevice(userId, accessToken.userId, accessToken.oauth_token, " ", deviceTypes.Where(x => x.Name.ToLower() == "garmin").FirstOrDefault().Id, true, accessToken.oauth_token_secret);
            }
            return RedirectToAction("Wearable", "Wearable");
        }
        #endregion Garmin

        #region Fitbit
        [Authorize]
        public ActionResult FitbitAuth()
        {
            var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var oAuth = new FitbitClient(baseURL + "/Wearable/FitbitCallback?");
            string authUrl = oAuth.GenerateAuthUrl(_appSettings.FitbitClientId);
            return Redirect(authUrl);
        }

        public ActionResult FitbitCallback()
        {
            var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var authenticator = new FitbitClient(baseURL + "/Wearable/FitbitCallback?");
            string code = Request.Query["code"];
            FitbitOAuth2 accessToken = Task.Run(() => authenticator.GetOAuth2Async(code, _appSettings.FitbitApiUrl, _appSettings.FitbitClientId, _appSettings.FitbitClientSecret)).Result;
            if (accessToken != null)
            {
                var deviceTypes = WearableUtility.GetWearableDevices((int)WearableDeviceType.Web);
                int userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                var scopeDetails = Task.Run(() => new FitbitClient().GetScopeDetails(accessToken.access_token, _appSettings.FitbitApiUrl)).Result;
                var fitbitUser = Task.Run(() => new FitbitClient().GetProfileDetails(accessToken.access_token, accessToken.user_id, _appSettings.FitbitApiUrl)).Result;
                int? offsetFromUTC = null;
                if (fitbitUser != null && fitbitUser.user != null)
                    offsetFromUTC = fitbitUser.user.offsetFromUTCMillis;
                if (scopeDetails != null && !string.IsNullOrEmpty(scopeDetails.scope))
                    WearableUtility.AddOrEditWearableDevice(userId, accessToken.user_id, accessToken.access_token, accessToken.refresh_token, deviceTypes.Where(x => x.Name.ToLower() == "fitbit").FirstOrDefault().Id, true, null, null, offsetFromUTC, scopeDetails.scope.Replace("=READ", "").ToLower());
                else
                {
                    LogReader logReader = new LogReader();
                    logReader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "FitbitCallback", null, "Scope detail was null : " + JsonConvert.SerializeObject(scopeDetails), null, null));
                }
            }
            return RedirectToAction("Wearable", "Wearable");
        }
        #endregion Fitbit

        #region Dexcom
        [Authorize]
        public ActionResult DexcomAuth()
        {
            var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var oAuth = new DexcomClient(baseURL + "/Wearable/DexcomCallback");
            string authUrl = oAuth.GenerateAuthUrl(_appSettings.DexcomApiUrl, _appSettings.DexcomClientId);
            return Redirect(authUrl);
        }

        public ActionResult DexcomCallback()
        {
            var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var authenticator = new DexcomClient(baseURL + "/Wearable/DexcomCallback");
            string code = Request.Query["code"];
            DexcomOAuth2 accessToken = Task.Run(() => authenticator.GetOAuth2Async(code, _appSettings.DexcomApiUrl, _appSettings.DexcomClientId, _appSettings.DexcomClientSecret)).Result;
            if (accessToken != null)
            {
                var deviceTypes = WearableUtility.GetWearableDevices((int)WearableDeviceType.Web);
                int userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                string externalUserId = new DexcomManager().FetchDeviceDetails(accessToken.access_token);
                if (!string.IsNullOrEmpty(externalUserId))
                    WearableUtility.AddOrEditWearableDevice(userId, externalUserId, accessToken.access_token, accessToken.refresh_token, deviceTypes.Where(x => x.Name.ToLower() == "dexcom").FirstOrDefault().Id, true, null);
            }
            return RedirectToAction("Wearable", "Wearable");
        }
        #endregion Dexcom

        #region Withings

        [Authorize]
        public ActionResult WithingsAuth()
        {
            var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var oAuth = new WithingsClient(baseURL + "/Wearable/WithingsCallback");
            string authUrl = oAuth.GenerateAuthUrl(_appSettings.WithingsAuthUrl, _appSettings.WithingsClientId, _appSettings.WithingsMode);
            return Redirect(authUrl);
        }

        public ActionResult WithingsCallback()
        {
            var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var authenticator = new WithingsClient(baseURL + "/Wearable/WithingsCallback");
            string code = Request.Query["code"];
            WithingsOAuth2 accessToken = Task.Run(() => authenticator.GetOAuth2Async(code, _appSettings.WithingsApiUrl, _appSettings.WithingsClientId, _appSettings.WithingsClientSecret)).Result;
            if (accessToken != null && accessToken.body != null)
            {
                var deviceTypes = WearableUtility.GetWearableDevices((int)WearableDeviceType.Web);
                int userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                WearableUtility.AddOrEditWearableDevice(userId, accessToken.body.userid, accessToken.body.access_token, accessToken.body.refresh_token, deviceTypes.Where(x => x.Name.ToLower() == "withings").FirstOrDefault().Id, true, null, null, null, accessToken.body.scope.Replace("user.", ""));
            }
            return RedirectToAction("Wearable", "Wearable");
        }
        #endregion Withings

        #region Omron
        [Authorize]
        public ActionResult OmronAuth()
        {
            var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var oAuth = new OmronClient(baseURL + "/Wearable/OmronCallback");
            string authUrl = oAuth.GenerateAuthUrl(_appSettings.OmronAuthUrl, _appSettings.OmronClientId);
            return Redirect(authUrl);
        }

        public ActionResult OmronCallback()
        {
            var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var authenticator = new OmronClient(baseURL + "/Wearable/OmronCallback");
            string code = Request.Query["code"];
            if (!string.IsNullOrEmpty(code))
            {
                OmronOAuth accessToken = Task.Run(() => authenticator.GetOAuth2Async(code, _appSettings.OmronApiUrl, _appSettings.OmronClientId, _appSettings.OmronClientSecret)).Result;
                if (accessToken != null)
                {
                    var deviceTypes = WearableUtility.GetWearableDevices((int)WearableDeviceType.Web);
                    JwtSecurityToken token = ValidateJWKToken(accessToken.id_token);
                    string externalUserId = token.Claims.First(c => c.Type == "sub").Value;
                    int userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                    if (!string.IsNullOrEmpty(externalUserId))
                        WearableUtility.AddOrEditWearableDevice(userId, externalUserId, accessToken.access_token, accessToken.refresh_token, deviceTypes.Where(x => x.Name.ToLower() == "omron").FirstOrDefault().Id, true, null);
                }
            }
            return RedirectToAction("Wearable", "Wearable");
        }

        public void OmronNotification(OmronNotification request)
        {
            LogReader logReader = new LogReader();
            logReader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "OmronNotification", null, "Request : " + JsonConvert.SerializeObject(request), null, null));
            if (request != null && !string.IsNullOrEmpty(request.type))
            {
                var device = WearableUtility.GetUserWearableDevicesByExternalUserId(request.id);
                if (device != null)
                {
                    if (request.type.Equals("bloodpressure") || request.type.Equals("activity") || request.type.Equals("weight"))
                    {
                        Task.Run(() => new OmronManager(_appSettings.SystemAdminId, _appSettings.OmronApiUrl, _appSettings.OmronClientId, _appSettings.OmronClientSecret, _appSettings.OmronRedirectUrl).FetchMeasurementLog(device, request.type));
                    }
                    else
                    {
                        logReader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "OmronNotification", null, "Request : " + JsonConvert.SerializeObject(request), null, null));
                    }
                }
            }
        }

        private static JwtSecurityToken ValidateJWKToken(string jwtToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(jwtToken);
                return jsonToken as JwtSecurityToken;
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                var debug = "Exception:" + ex.Message + ex.InnerException;
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Wearable.ValidateJWKToken", null, debug, null, ex);
                reader.WriteLogMessage(logEvent);
            }
            return null;
        }
        #endregion Omron

        #region GoogleFit

        [Authorize]
        public ActionResult GoogleFitAuth()
        {
            var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var oAuth = new GoogleFitClient(baseURL + "/Wearable/GoogleFitCallback");
            string authUrl = oAuth.GenerateAuthUrl(_appSettings.GoogleFitClientId);
            return Redirect(authUrl);
        }

        public ActionResult GoogleFitCallback()
        {
            string code = Request.Query["code"];
            string error = Request.Query["error"];
            if (!string.IsNullOrEmpty(code))
            {
                var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                var authenticator = new GoogleFitClient(baseURL + "/Wearable/GoogleFitCallback");
                GoogleFitOAuth accessToken = Task.Run(() => authenticator.GetOAuth2Async(code, _appSettings.GoogleFitClientId, _appSettings.GoogleFitClientSecret)).Result;
                if (accessToken != null)
                {
                    var deviceTypes = WearableUtility.GetWearableDevices((int)WearableDeviceType.Web);
                    int userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                    var googleFitUser = Task.Run(() => authenticator.GetProfileDetails(accessToken.access_token)).Result;
                    if (googleFitUser != null)
                    {
                        WearableUtility.AddOrEditWearableDevice(userId, googleFitUser.id, accessToken.access_token, accessToken.refresh_token, deviceTypes.Where(x => x.Name.ToLower() == "google fit").FirstOrDefault().Id, true, null);
                    }
                    else
                    {
                        LogReader logReader = new LogReader();
                        logReader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "GoogleFitCallback", null, "Response profile invalid.", null, null));
                    }
                }
            }
            if (!string.IsNullOrEmpty(error))
            {
                LogReader logReader = new LogReader();
                logReader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "GoogleFitCallback", null, "Response error : " + error, null, null));
            }
            return RedirectToAction("Wearable", "Wearable");
        }
        #endregion GoogleFit
    }
}
