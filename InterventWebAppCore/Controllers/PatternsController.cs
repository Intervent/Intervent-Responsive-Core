using Intervent.Business;
using Intervent.DAL;
using Intervent.HWS;
using Intervent.Web.DataLayer;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;

namespace InterventWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PatternsController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private IntuityManager intuityManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatternsController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            intuityManager = new IntuityManager(_appSettings.IntuityDTCOrgCode, _appSettings.EbenOrgCode, _appSettings.SystemAdminId);
        }

        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public IActionResult PairPogo(string clientId, string userId, [FromQuery] string date, [FromBody] PairPogoRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");
            string error = ValidateRequest(userId, request.user_id);
            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);
            if (request.devices == null || request.devices.Count() <= 0)
                return BadRequest("Device not found");

            var response = intuityManager.PairPogo(request);
            LogResponse(response, "Pairing POGO for User ID : " + request.user_id, true);
            if (response.Status)
                return Ok();
            else
                return BadRequest(response.ErrorMsg);
        }

        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public IActionResult PostValidateUserResponse(string clientId, string userId, [FromQuery] string date, [FromBody] VerifyIntuityUserResponse response)
        {
            try
            {
                if (response == null)
                    return BadRequest("Invalid request");

                var eligibilityResponse = intuityManager.VerifyUserResponse(_userManager, response);
                LogResponse(eligibilityResponse, "Post Validate User Response through API for User ID : " + response.user_id + ", Request : " + JsonConvert.SerializeObject(response), true);

                if (eligibilityResponse.Status)
                    return Ok();
                else
                    return BadRequest(eligibilityResponse.ErrorMsg);
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                var msg = string.Format("Error in PostValidateUser. User:{0}; Msg:{1}", userId, ex.Message);
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "PatternsController.PostValidateUser", null, msg, null, ex);
                reader.WriteLogMessage(logEvent);
            }

            return StatusCode(500, "An internal server error occurred.");
        }

        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public IActionResult PostProfile(string clientId, string userId, [FromQuery] string date, [FromBody] ProfileUpdate request)
        {
            if (request == null)
                return BadRequest("Invalid request");
            string error = ValidateRequest(userId, request.user_id);
            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            var response = intuityManager.UpdateProfile(request);
            LogResponse(response, "Post Profile through API for User ID : " + userId, true);

            if (response.Status)
            {
                return Ok();
            }
            else
            {
                return BadRequest(response.ErrorMsg);
            }
        }

        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public IActionResult PostPogo(string clientId, string userId, [FromQuery] string date, [FromBody] ReadingPogoRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request");
                string error = ValidateRequest(userId, request.user_id);
                if (!string.IsNullOrEmpty(error))
                    return BadRequest(error);

                var response = intuityManager.ReadPogo(request);
                LogResponse(response, "Post Pogo through API for User ID : " + userId, false);

                if (response.Status)
                    return Ok();
                else
                    return BadRequest(response.ErrorMsg);
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                var msg = string.Format("Error in PostPogo. User:{0}; Msg:{1}", userId, ex.Message);
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "PatternsController.PostPogo", null, msg, null, ex);
                reader.WriteLogMessage(logEvent);
            }

            return StatusCode(500, "An internal server error occurred.");
        }

        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public IActionResult SaveFood(string clientId, string userId, [FromQuery] string date, [FromBody] EXT_FoodRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");
            string error = ValidateRequest(userId, request.user_id);
            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            var response = intuityManager.AddEXTFoodData(request);

            if (response)
            {
                return Ok();
            }
            else
            {
                return BadRequest("UserId does not match");
            }
        }

        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public IActionResult SaveWeight(string clientId, string userId, [FromQuery] string date, [FromBody] EXT_WeightRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");
            string error = ValidateRequest(userId, request.user_id);
            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            var response = intuityManager.AddEXTWeightData(request);

            if (response)
            {
                return Ok();
            }
            else
            {
                return BadRequest("UserId does not match");
            }
        }

        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public IActionResult SaveSleep(string clientId, string userId, [FromQuery] string date, [FromBody] EXT_SleepRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");
            string error = ValidateRequest(userId, request.user_id);
            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            var response = intuityManager.AddEXTSleepData(request);
            if (response)
            {
                return Ok();
            }
            else
            {
                return BadRequest("UserId does not match");
            }
        }

        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public IActionResult SaveSummary(string clientId, string userId, [FromQuery] string date, [FromBody] EXT_SummaryRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");
            string error = ValidateRequest(userId, request.user_id);
            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            var response = intuityManager.AddEXTSummaryData(request);
            if (response)
            {
                return Ok();
            }
            else
            {
                return BadRequest("UserId does not match");
            }
        }

        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public IActionResult SaveWorkout(string clientId, string userId, [FromQuery] string date, [FromBody] EXT_WorkoutRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");
            string error = ValidateRequest(userId, request.user_id);
            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            var response = intuityManager.AddEXTWorkoutData(request);
            if (response)
            {
                return Ok();
            }
            else
            {
                return BadRequest("UserId does not match");
            }
        }

        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public IActionResult SaveBloodPressure(string clientId, string userId, [FromQuery] string date, [FromBody] EXT_BloodPressureRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");
            string error = ValidateRequest(userId, request.user_id);
            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            var response = intuityManager.AddEXTBloodPressureData(request);
            if (response)
            {
                return Ok();
            }
            else
            {
                return BadRequest("UserId does not match");
            }
        }

        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public IActionResult VerifyEligibility(string clientId, string userId, [FromQuery] string date, [FromBody] IntuityEligibilityRequest request)
        {

            if (request == null)
                return BadRequest("Invalid request");
            if (string.IsNullOrEmpty(request.unique_id))
                return BadRequest("UniqueId not found");
            if (request.unique_id.ToString() != userId)
                return BadRequest("UniqueId does not match");

            IntuityEligibilityResponse response = intuityManager.ProcessIntuityEligibility(request);
            LogResponse(response, "Verify Eligibility through API for Unique ID : " + request.unique_id, true);
            if (response.Status)
                return Ok(response);
            else
                return BadRequest(response.ErrorMsg);
        }

        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public IActionResult UpdateAccount(string clientId, string userId, [FromQuery] string date, [FromBody] AccountUpdate request)
        {

            if (request == null)
                return BadRequest("Invalid request");
            if (request.user_id == 0)
                return BadRequest("UserId not found");
            if (request.user_id.ToString() != userId)
                return BadRequest("UserId does not match");
            string error = ValidateRequest(userId, request.user_id);
            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);

            var response = intuityManager.UpdateAccount(_userManager, request);

            if (response.Status)
                return Ok(response);
            else
                return BadRequest(response.ErrorMsg);
        }

        private string ValidateRequest(string userId, int? user_id)
        {
            if (string.IsNullOrEmpty(userId) || !user_id.HasValue)
                return "UserId not found";
            if (!user_id.ToString().Equals(userId))
                return "UserId does not match";
            if (!ExternalUtility.IsValidIntuityUserId(user_id.Value, null))
                return "UserId does not exist";
            return "";
        }

        private void LogResponse(ProcessResponse response, string message, bool isLog)
        {
            try
            {
                if (!response.Status)
                {
                    LogReader reader = new LogReader();
                    NLog.LogLevel logLevel = NLog.LogLevel.Error;
                    message += string.Format(" But it failed. StatusCode : {0}, ErrorMsg : {1}, Request : {2}", response.StatusCode, response.ErrorMsg, response.Request);
                    var logEvent = new LogEventInfo(logLevel, "PatternsController", null, message, null, response.Exception);
                    reader.WriteLogMessage(logEvent);
                }
                else if (isLog)
                {
                    LogReader reader = new LogReader();
                    NLog.LogLevel logLevel = NLog.LogLevel.Info;
                    message += string.Format(" Success. Request : {0}", response.Request);
                    var logEvent = new LogEventInfo(logLevel, "PatternsController", null, message, null, response.Exception);
                    reader.WriteLogMessage(logEvent);
                }
            }
            catch { }
        }

    }
}
