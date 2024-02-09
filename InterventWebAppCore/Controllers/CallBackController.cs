using Intervent.Web.DataLayer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace InterventWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CallBackController : ControllerBase
    {
        [HttpPost]
        public void CallBackIntervent([FromBody] object request)
        {
            try
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Info, "CallBackIntervent", null, JsonConvert.SerializeObject(request), null, null);
                logReader.WriteLogMessage(logEvent);
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "CallBackIntervent", null, "CallBackIntervent : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
        }

    }
}