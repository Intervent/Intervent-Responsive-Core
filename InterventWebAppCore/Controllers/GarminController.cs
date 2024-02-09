using Intervent.HWS.Model;
using Intervent.HWS.Model.Wearable.Garmin;
using Intervent.Web.DataLayer;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;

namespace InterventWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GarminController : ControllerBase
    {
        public static string SOURCE = "Garmin";

        public static string INPUT_METHOD = "API";

        private readonly AppSettings _appSettings;

        public GarminController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [HttpPost("{request?}")]
        public IActionResult PostActivityLog(GarminActivityRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request");

                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Info, "GarminController.PostActivityLog_Request:", null, JsonConvert.SerializeObject(request), null);
                reader.WriteLogMessage(logEvent);
                ExternalUtility.AddActivity(request, SOURCE, INPUT_METHOD);
                return Ok();
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "GarminController.PostActivityLog", null, JsonConvert.SerializeObject(request), null, ex);
                reader.WriteLogMessage(logEvent);
            }

            return StatusCode(500, "An internal server error occurred.");
        }

        [HttpPost("{request?}")]
        public IActionResult PostWeightLog(GarminWeightRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request");

                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Info, "GarminController.PostWeightLog_Request:", null, JsonConvert.SerializeObject(request), null);
                reader.WriteLogMessage(logEvent);
                ExternalUtility.AddWeight(request, SOURCE, INPUT_METHOD, _appSettings.SystemAdminId);
                return Ok();
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "GarminController.PostWeightLog", null, JsonConvert.SerializeObject(request), null, ex);
                reader.WriteLogMessage(logEvent);
            }

            return StatusCode(500, "An internal server error occurred.");
        }

        [HttpPost("{request?}")]
        public IActionResult PostBloodPressureLog(GarminBloodPressureRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request");

                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Info, "GarminController.PostBloodPressureLog_Request:", null, JsonConvert.SerializeObject(request), null);
                reader.WriteLogMessage(logEvent);
                ExternalUtility.AddBloodPressure(request, SOURCE, INPUT_METHOD);
                return Ok();
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "GarminController.PostBloodPressureLog", null, JsonConvert.SerializeObject(request), null, ex);
                reader.WriteLogMessage(logEvent);
            }

            return StatusCode(500, "An internal server error occurred.");
        }

        [HttpPost("{request?}")]
        public IActionResult PostSleepLog(GarminSleepRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request");

                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Info, "GarminController.PostSleepLog_Request:", null, JsonConvert.SerializeObject(request), null);
                reader.WriteLogMessage(logEvent);
                ExternalUtility.AddSleep(request, SOURCE, INPUT_METHOD);
                return Ok();
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "GarminController.PostSleepLog", null, JsonConvert.SerializeObject(request), null, ex);
                reader.WriteLogMessage(logEvent);
            }

            return StatusCode(500, "An internal server error occurred.");
        }

        [HttpPost("{request?}")]
        public IActionResult PostDailiesLog(GarminDailyDataRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request");

                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Info, "GarminController.PostDailiesLog_Request:", null, JsonConvert.SerializeObject(request), null);
                reader.WriteLogMessage(logEvent);
                ExternalUtility.AddSummary(request, SOURCE, INPUT_METHOD);
                return Ok();
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "GarminController.PostDailiesLog", null, JsonConvert.SerializeObject(request), null, ex);
                reader.WriteLogMessage(logEvent);
            }

            return StatusCode(500, "An internal server error occurred.");
        }

        [HttpPost("{request?}")]
        public IActionResult Deregistration(GarminDeregistrationUsers request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request");

                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Info, "GarminController.Deregistration_Request:", null, JsonConvert.SerializeObject(request), null);
                reader.WriteLogMessage(logEvent);
                ExternalUtility.GarminDeregistration(request);
                return Ok();
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "GarminController.Deregistration", null, JsonConvert.SerializeObject(request), null, ex);
                reader.WriteLogMessage(logEvent);
            }

            return StatusCode(500, "An internal server error occurred.");
        }
    }
}
