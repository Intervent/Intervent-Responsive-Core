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
    [Route("api/[controller]")]
    public class PlacerController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        PlacerManager placerManager = new PlacerManager();

        public PlacerController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        [RequireHttps]
        public IActionResult CreateEligibility(string clientId, string userId, [FromBody] PlacerParticipantRequest request)
        {
            try
            {
                var message = "";
                var requestLog = "CreateEligibility Request : " + JsonConvert.SerializeObject(request) + ". Response : ";
                if (request == null)
                    message = "Invalid request";
                else if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(request.participant_id))
                    message = "ParticipantId not found";
                else if (!request.participant_id.ToString().Equals(userId))
                    message = "ParticipantId does not match";
                else if (string.IsNullOrEmpty(request.first_name))
                    message = "first_name can't be empty";
                else if (string.IsNullOrEmpty(request.last_name))
                    message = "last_name can't be empty";

                if (!string.IsNullOrEmpty(message))
                {
                    LogRequestResponse(requestLog + message);
                    return BadRequest(message);
                }

                var response = placerManager.CreateEligibility(_userManager, request, _appSettings.TeamsBPOrgId);
                LogRequestResponse(requestLog + response);
                if (response.Equals("success"))
                    return Ok(response);
                else
                    return BadRequest(response);
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "PlacerController", null, "CreateEligibility : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return BadRequest();
            }
        }

        private void LogRequestResponse(string message)
        {
            LogReader logReader = new LogReader();
            var logEvent = new LogEventInfo(NLog.LogLevel.Info, "PlacerController", null, message, null, null);
            logReader.WriteLogMessage(logEvent);
        }
    }
}