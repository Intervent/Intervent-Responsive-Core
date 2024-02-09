using Intervent.HWS;
using Intervent.Web.DataLayer;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;

namespace InterventWebApp.Controllers
{
    [ApiController]
    [Route("WebForm/[controller]")]
    public class WebFormApiController : ControllerBase
    {
        private readonly AppSettings _appSettings;

        public WebFormApiController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [HttpPost("{request?}")]
        public IActionResult SaveWebForm(WebFormRequest request)
        {
            var message = "Invalid request";
            var requestLog = "SaveWebForm Request : " + JsonConvert.SerializeObject(request) + ". Response : ";
            if (request == null || string.IsNullOrEmpty(request.in_email_id) || string.IsNullOrEmpty(request.in_first_name) || string.IsNullOrEmpty(request.in_last_name) || string.IsNullOrEmpty(request.in_phone_no) || request.in_inquiry_type == 0 || string.IsNullOrEmpty(request.in_client_code))
            {
                LogRequestResponse(requestLog + message);
                return BadRequest(message);
            }
            if (request.in_pogo_monitor && string.IsNullOrEmpty(request.in_monitor_number))
            {
                message = "Serial Number not found";
                LogRequestResponse(requestLog + message);
                return BadRequest(message);
            }
            if (CRMUtility.GetInquiryTypes().InquiryTypes.Where(x => x.Id == request.in_inquiry_type).Count() == 0)
            {
                message = "Inquiry Type not found";
                LogRequestResponse(requestLog + message);
                return BadRequest(message);
            }
            var response = CRMUtility.ProcessWebForm(request, _appSettings.IntuityOrgId, _appSettings.SystemAdminId);
            if (response.success)
            {
                LogRequestResponse(requestLog + "Form saved successfully");
                return Ok("Thank you. We have received your message. Our customer support team will get back to you via email within 2-4 business days.");
            }
            else
            {
                LogRequestResponse(requestLog + "Try after some time");
                return BadRequest("Try after some time");
            }
        }

        private void LogRequestResponse(string message)
        {
            LogReader logReader = new LogReader();
            var logEvent = new LogEventInfo(NLog.LogLevel.Info, "WebFormApiController", null, message, null, null);
            logReader.WriteLogMessage(logEvent);
        }


        [HttpGet("{in_client_code?}")]
        public IActionResult GetInquiryTypes(string in_client_code)
        {

            var requestLog = "WebForm GetInquiryTypes Request : " + in_client_code + ". Response : ";
            if (!string.IsNullOrEmpty(in_client_code))
            {
                var inquiryTypes = CRMUtility.GetInquiryTypes();
                LogRequestResponse(requestLog + "Success. " + JsonConvert.SerializeObject(inquiryTypes));
                return new JsonResult(new { Success = true, Record = inquiryTypes });
            }
            else
            {
                LogRequestResponse(requestLog + "Failed");
                return new JsonResult(new { Success = false });
            }
        }
    }
}
