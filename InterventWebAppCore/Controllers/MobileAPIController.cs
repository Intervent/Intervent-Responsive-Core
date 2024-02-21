using Intervent.Business;
using Intervent.DAL;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace InterventWebApp.Controllers
{
    [ApiController]
    [Route("Mobile/[controller]/[action]")]
    public class MobileAPIController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public MobileAPIController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings, IHostEnvironment environment)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _environment = environment;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("Dashboard")]
        [HttpPost]
        public IActionResult DashBoard()
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                var response = MobileUtility.GetDashBoardModel(userIdentity.UserId, userIdentity.TimeZone, userIdentity.ExpirationUrl, _appSettings.EmailUrl);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("Profile")]
        [HttpPost]
        public IActionResult Profile()
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                var response = MobileUtility.GetUserProfile(userIdentity.UserId, userIdentity.DeviceId, _appSettings.EmailUrl);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("Feed")]
        [HttpPost]
        public IActionResult Feed()
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                var response = MobileUtility.GetUserFeeds(userIdentity.UserId, userIdentity.TimeZone, userIdentity.TimeZoneName, _appSettings.EmailUrl);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("Device")]
        [HttpPost]
        public IActionResult Device()
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                return Ok(new DeviceResponse());
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("Wearable")]
        [HttpPost]
        public IActionResult Wearable()
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                var response = MobileUtility.GetWearableDetails(userIdentity.UserId, userIdentity.DeviceId, _appSettings.EmailUrl);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("Notification")]
        [HttpPost]
        public IActionResult Notification()
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                var response = MobileUtility.GetUserNotifications(userIdentity.UserId, userIdentity.TimeZone, userIdentity.TimeZoneName, _appSettings.EmailUrl);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("UpdateDashboardMessage")]
        [HttpPost("{request?}")]
        public IActionResult UpdateDashboardMessage(DashboardMessageRequest request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0 && request != null && request.message_id != 0)
            {
                var response = ParticipantUtility.UpdateDashboardMessage(request.message_id, null, null, false, null);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("UpdateProfile")]
        [HttpPost("{request?}")]
        public async Task<IActionResult> UpdateProfile(UserProfileRequest request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0 && request != null)
            {
                var result = await MobileUtility.UpdateUserProfile(_userManager, userIdentity.UserId, request, userIdentity.DeviceId);
                if (result.Succeeded)
                    return Ok(new { status = result.Succeeded, message = "Successfully updated" });
                else
                    return Ok(new { status = result.Succeeded, message = result.error != null ? result.error : null });
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("UpdateSettings")]
        [HttpPost("{request?}")]
        public async Task<IActionResult> UpdateSettings(UpdateSettings request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0 && request != null)
            {
                if (string.IsNullOrEmpty(userIdentity.Token))
                    return BadRequest("Push Notification token can't be empty.");

                request.notification_token = request.mobile_notification ? userIdentity.Token : null;
                var result = await MobileUtility.UpdateUserSettings(_userManager, userIdentity.UserId, request, userIdentity.DeviceId);
                if (result.Succeeded)
                    return Ok(new { status = result.Succeeded, message = "Successfully updated" });
                else
                    return Ok(new { status = result.Succeeded, message = result.error != null ? result.error : null });
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("ChangePassword")]
        [HttpPost("{request?}")]
        public async Task<IActionResult> ChangePassword(ChangePassword request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0 && request != null && !string.IsNullOrEmpty(request.new_password) && !string.IsNullOrEmpty(request.old_password))
            {
                request.new_password = Encoding.ASCII.GetString(Convert.FromBase64String(request.new_password)).Substring(6);
                request.old_password = Encoding.ASCII.GetString(Convert.FromBase64String(request.old_password)).Substring(6);

                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasLowerChar = new Regex(@"[a-z]+");

                if (request.new_password.Length < 8)
                    return Ok(new { status = false, message = "Passwords must be a minimum of 8 characters in length." });
                else if (!hasNumber.IsMatch(request.new_password))
                    return Ok(new { status = false, message = "Password should contain at least one numeric value." });
                else if (!hasUpperChar.IsMatch(request.new_password))
                    return Ok(new { status = false, message = "Password should contain at least one upper case letter." });
                else if (!hasLowerChar.IsMatch(request.new_password))
                    return Ok(new { status = false, message = "Password should contain at least one lower case letter." });

                var msg = await MobileUtility.ChangePassword(_userManager, userIdentity, request);
                return Ok(new { status = msg.Contains("password has been changed"), message = msg });
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("VerifyPassword")]
        [HttpPost("{request?}")]
        public async Task<IActionResult> VerifyPassword(VerifyPassword request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0 && request != null && !string.IsNullOrEmpty(request.password))
            {
                request.password = Encoding.ASCII.GetString(Convert.FromBase64String(request.password)).Substring(6);
                var response = await MobileUtility.VerifyPassword(_userManager, userIdentity, request);
                return Ok(new { status = response.status, message = response.error });
            }
            else
                return BadRequest("Invalid request");
        }

        [ActionName("ForgotPassword")]
        [HttpPost("{request?}")]
        public async Task<IActionResult> ForgotPassword(ForgotPassword request)
        {
            if (request != null && !string.IsNullOrEmpty(request.email))
            {
                var response = await AccountUtility.ForgotPassword(_userManager, _environment.ContentRootPath, request.email, "info@myintervent.com", _appSettings.InfoEmail, _appSettings.SecureEmail, _appSettings.SMPTAddress, _appSettings.PortNumber, _appSettings.SecureEmailPassword, _appSettings.MailAttachmentPath);
                return Ok(new { status = response.Contains("success"), message = response });
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("Logout")]
        [HttpPost]
        public IActionResult Logout()
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                AccountUtility.LogoutUser(userIdentity.UserId, userIdentity.DeviceId);
                // FormsAuthentication.SignOut();
            }
            return Ok();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("DailyVitals")]
        [HttpGet]
        public IActionResult DailyVitals()
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                var response = MobileUtility.GetDailyVitals(userIdentity.UserId, userIdentity.TimeZone);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("SaveDailyVitals")]
        [HttpPost("{request?}")]
        public IActionResult SaveDailyVitals(SaveDailyVitals request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0 && request != null && !string.IsNullOrEmpty(request.question_no) && !string.IsNullOrEmpty(request.answer))
            {
                var response = MobileUtility.SaveDailyVitals(userIdentity.UserId, userIdentity.TimeZone, request);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("ChangeEmail")]
        [HttpPost("{request?}")]
        public async Task<IActionResult> ChangeEmail(ChangeEmail request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0 && request != null && !string.IsNullOrEmpty(request.email))
            {
                var response = await MobileUtility.ChangeEmail(_userManager, userIdentity.UserId, request, userIdentity.DeviceId);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("States")]
        [HttpPost("{request?}")]
        public IActionResult States(StateRequest request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0 && request != null && request.country_id != 0)
            {
                var response = MobileUtility.ListStates(request).states;
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("Countries")]
        [HttpGet]
        public IActionResult Countries()
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                var response = MobileUtility.ListCountries().countries;
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("MessageDashBoard")]
        [HttpPost("{request?}")]
        public IActionResult MessageDashBoard(DashBoardMessageRequest request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0 && request != null)
            {
                var response = MobileUtility.GetDashBoardMessages(userIdentity.UserId, request, userIdentity.TimeZone, _appSettings.SouthUniversityOrgId);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "InternalServerError");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("Message")]
        [HttpPost("{request?}")]
        public IActionResult Message(MessageRequest request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0 && request != null && request.message_id != 0)
            {
                var response = MobileUtility.GetMessages(userIdentity.UserId, request.message_id, userIdentity.TimeZone, _appSettings.SystemAdminId, _appSettings.EmailUrl);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("AddMessage")]
        [HttpPost]
        public async Task<IActionResult> AddMessage()
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);

            if (userIdentity != null && userIdentity.UserId != 0 /*&& Request.Content.IsMimeMultipartContent()*/)
            {
                AddMessageRequest request = new AddMessageRequest();
                string filePath = _environment.ContentRootPath + "~/Messageuploads/";
                var provider = new MultipartFormDataStreamProvider("filePath");
               // await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var data in provider.Contents)
                {
                    foreach (var p in data.Headers.ContentDisposition.Parameters)
                    {
                        if (p.Value.Contains("parent_message_id"))
                        {
                            request.parent_message_id = Convert.ToInt32(await data.ReadAsStringAsync());
                        }
                        else if (p.Value.Contains("message_id"))
                        {
                            request.message_id = Convert.ToInt32(await data.ReadAsStringAsync());
                        }
                        else if (p.Value.Contains("subject"))
                        {
                            request.subject = await data.ReadAsStringAsync();
                        }
                        else if (p.Value.Contains("message"))
                        {
                            request.message = await data.ReadAsStringAsync();
                        }
                        else if (p.Value.Contains("is_sent"))
                        {
                            request.is_sent = Convert.ToBoolean(await data.ReadAsStringAsync());
                        }
                    }
                }

                foreach (MultipartFileData file in provider.FileData)
                {
                    if (file != null)
                    {
                         var targetpath = _environment.ContentRootPath + "~/Messageuploads/";
                         string fileName = file.Headers.ContentDisposition.FileName;
                         if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                             fileName = fileName.Trim('"');
                         if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                             fileName = Path.GetFileName(fileName);
                         var postedFileExtension = Path.GetExtension(fileName);
                         if (postedFileExtension.ToLower().Equals(".pdf") || postedFileExtension.ToLower().Equals(".jpg") || postedFileExtension.ToLower().Equals(".png") || postedFileExtension.ToLower().Equals(".jpeg") || postedFileExtension.ToLower().Equals(".gif"))
                         {
                             var fileNameNew = DateTime.Now.ToString("_ddMMyyhhmmssFFF") + postedFileExtension;
                             System.IO.File.Move(file.LocalFileName, Path.Combine(targetpath, fileNameNew));
                             request.attachement_name = fileNameNew;
                             if (string.IsNullOrEmpty(request.message))
                                 request.message = fileNameNew;
                         }
                         else
                         {
                             if (file != null && !string.IsNullOrEmpty(file.LocalFileName))
                                System.IO.File.Delete(file.LocalFileName);
                         }
                    }
                }

                if (string.IsNullOrEmpty(request.message))
                    return BadRequest("Message body can't be empty");

                var response = MobileUtility.AddMessage(userIdentity.UserId, _appSettings.SystemAdminId, request, userIdentity.TimeZone, userIdentity.RoleCode, _appSettings.EmailUrl);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("DeleteAttachment")]
        [HttpPost("{request?}")]
        public IActionResult DeleteAttachment(DeleteMessageRequest request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0 && request != null && !string.IsNullOrEmpty(request.attachement_name) && request.message_id != 0)
            {
                try
                {
                    if (MobileUtility.DeleteAttachment(userIdentity.UserId, request, _appSettings.SystemAdminId))
                        return Ok(new { status = true, message = "Successfully updated" });
                    else
                        return Ok(new { status = false, message = "Error" });
                }
                catch (Exception ex)
                {
                    LogReader logReader = new LogReader();
                    var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userIdentity.UserId + ")", null, "DeleteAttachment : " + ex.Message, null, ex);
                    logReader.WriteLogMessage(logEvent);
                    return StatusCode(500, "An internal server error occurred.");
                }
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("UploadProfilePicture")]
        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture()
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            try
            {
                if (userIdentity != null && userIdentity.UserId != 0 && Request.Form.Files.Any())
                {
                    string filePath = _environment.ContentRootPath + "~/ProfilePictures/";
                    var provider = new MultipartFormDataStreamProvider(filePath);
                    //await Request.Content.ReadAsMultipartAsync(provider);

                    if (provider.FileData.Count > 0)
                    {
                        var file = provider.FileData[0];
                        if (file != null && CheckIfImage(file))
                        {
                            var targetpath = _environment.ContentRootPath + "~/ProfilePictures/";
                            string fileName = file.Headers.ContentDisposition.FileName;
                            if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                                fileName = fileName.Trim('"');
                            if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                                fileName = Path.GetFileName(fileName);
                            var postedFileExtension = Path.GetExtension(fileName);
                            var fileNameNew = DateTime.Now.ToString("_ddMMyyhhmmssFFF") + postedFileExtension;
                            System.IO.File.Move(file.LocalFileName, Path.Combine(targetpath, fileNameNew));
                            await AccountUtility.UploadPicture(_userManager, userIdentity.UserId, fileNameNew, targetpath);
                            return Ok(new { status = true, message = "Successfully updated" });
                        }
                        else
                        {
                            if (file != null && !string.IsNullOrEmpty(file.LocalFileName))
                                System.IO.File.Delete(file.LocalFileName);
                        }
                    }
                    return BadRequest("Invalid request");
                }
                else
                    return BadRequest("Invalid request");
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userIdentity.UserId + ")", null, "UploadProfilePicture : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("WatchVideo")]
        [HttpGet]
        public IActionResult WatchVideo()
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                var response = MobileUtility.GetWatchVideo(userIdentity.UserId, _appSettings.EmailUrl);
                if (response != null)
                    return Ok(response);
                else
                    return StatusCode(500, "An internal server error occurred.");
            }
            else
                return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("UpdateDeviceStatus")]
        [HttpPost("{request?}")]
        public IActionResult UpdateDeviceStatus(DeviceRequest request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                LogReader logReader = new LogReader();
                try
                {
                    if (request != null && !string.IsNullOrEmpty(request.device_type) && !string.IsNullOrEmpty(request.external_user_id))
                    {
                        logReader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Info, "Mobile API - UserId (" + userIdentity.UserId + ")", null, "UpdateDeviceStatus : " + JsonConvert.SerializeObject(request), null, null));
                        var devicetypes = WearableUtility.GetWearableDevices((int)WearableDeviceType.Mobile);
                        if (request.device_type.ToLower() == "google")
                        {
                            WearableUtility.AddOrEditWearableDevice(userIdentity.UserId, request.external_user_id, null, null, devicetypes.Where(x => x.Name.ToLower() == "google fit").FirstOrDefault().Id, request.is_connected, null, userIdentity.DeviceId);
                            return Ok();
                        }
                        else if (request.device_type.ToLower() == "apple")
                        {
                            WearableUtility.AddOrEditWearableDevice(userIdentity.UserId, request.external_user_id, null, null, devicetypes.Where(x => x.Name.ToLower() == "apple health").FirstOrDefault().Id, request.is_connected, null, userIdentity.DeviceId);
                            return Ok();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userIdentity.UserId + ")", null, "UpdateDeviceDetails : " + ex.Message, null, ex);
                    logReader.WriteLogMessage(logEvent);
                    return StatusCode(500, "An internal server error occurred.");
                }
            }
            return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("GoogleFitData")]
        [HttpPost("{request?}")]
        public IActionResult GoogleFitData(GoogleFitness request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                LogReader logReader = new LogReader();
                try
                {
                    Task.Run(() => WriteToFileAsync(userIdentity.UserId, DateTime.UtcNow + " : GoogleFitData UserId (" + userIdentity.UserId + ") : " + JsonConvert.SerializeObject(request)));
                    if (request != null)
                    {
                        request.Status = true;
                        new GoogleFitManager(_appSettings.SystemAdminId).ProcessGoogleFitnessdata(userIdentity.UserId, request);
                        return Ok("Updated successfully.");
                    }
                }
                catch (Exception ex)
                {
                    logReader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userIdentity.UserId + ")", null, "Error GoogleFitData : " + ex.Message, null, ex));
                    return StatusCode(500, "An internal server error occurred.");
                }
            }
            return BadRequest("Invalid request");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("AppleHealthData")]
        [HttpPost("{request?}")]
        public IActionResult AppleHealthData(AppleHealth request)
        {
            var userIdentity = MobileUtility.GetUserSession((ClaimsIdentity)User.Identity);
            if (userIdentity != null && userIdentity.UserId != 0)
            {
                LogReader logReader = new LogReader();
                try
                {
                    Task.Run(() => WriteToFileAsync(userIdentity.UserId, DateTime.UtcNow + " : AppleHealthData UserId (" + userIdentity.UserId + ") : " + JsonConvert.SerializeObject(request)));
                    if (request != null)
                    {
                        new AppleHealthManager().ProcessAppleHealthdata(userIdentity.UserId, userIdentity.TimeZoneName, request, _appSettings.SystemAdminId);
                        return Ok("Updated successfully.");
                    }
                }
                catch (Exception ex)
                {
                    logReader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userIdentity.UserId + ")", null, "Error AppleHealthData : " + ex.Message, null, ex));
                    return StatusCode(500, "An internal server error occurred.");
                }
            }
            return BadRequest("Invalid request");
        }

        private async Task WriteToFileAsync(int userId, string log)
        {
            LogReader logReader = new LogReader();
            try
            {
                string FilePath = _appSettings.MobileWearableLogPath;
                var fileName = "Mobile_Wearable_Log_" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
                byte[] encodedText = Encoding.Unicode.GetBytes(log + "\n");
                string filePath = FilePath + fileName;
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                using (FileStream sourceStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                };
            }
            catch (Exception e)
            {
                logReader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userId + ")", null, e.Message, null, e));
            }
        }

        public static bool IsFileLocked(string filePath)
        {
            bool lockStatus = false;
            try
            {
                using (FileStream fileStream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    lockStatus = !fileStream.CanWrite;
                }
            }
            catch
            {
                lockStatus = true;
            }
            return lockStatus;
        }

        private bool CheckIfImage(MultipartFileData postedFile)
        {
            int ImageMinimumBytes = 512;
            string fileName = postedFile.Headers.ContentDisposition.FileName;
            if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                fileName = fileName.Trim('"');
            if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                fileName = Path.GetFileName(fileName);
            var postedFileExtension = Path.GetExtension(fileName);
            if (string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(postedFile.Headers.ContentType.MediaType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
           !string.Equals(postedFile.Headers.ContentType.MediaType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
           !string.Equals(postedFile.Headers.ContentType.MediaType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
           !string.Equals(postedFile.Headers.ContentType.MediaType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
           !string.Equals(postedFile.Headers.ContentType.MediaType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
           !string.Equals(postedFile.Headers.ContentType.MediaType, "image/png", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                try
                {
                    var fileBuf = System.IO.File.ReadAllBytes(postedFile.LocalFileName);
                    if (postedFile.Headers.ContentDisposition.Size < ImageMinimumBytes)
                    {
                        return false;
                    }

                    byte[] buffer = new byte[ImageMinimumBytes];
                    buffer = fileBuf;
                    string content = System.Text.Encoding.UTF8.GetString(buffer);
                    if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

                try
                {
                    using (var bitmap = new System.Drawing.Bitmap(postedFile.LocalFileName))
                    {
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
            return true;
        }
    }
}
