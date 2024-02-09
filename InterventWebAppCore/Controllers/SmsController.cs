using Intervent.Web.DataLayer;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NLog;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace InterventWebApp
{
    public class SmsController : TwilioController
    {
        private readonly AppSettings _appSettings;

        public SmsController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        // GET: GetTextResponse
        [HttpPost]
        public TwiMLResult PostSmsReply(SmsRequest incomingMessage)
        {
            var messagingResponse = new MessagingResponse();
            string authToken = _appSettings.TwilioAuthToken;

            if (!RequestValidationHelper.IsValidRequest(HttpContext, authToken))
            {
                LogReader errorReader = new LogReader();
                var errorEvent = new LogEventInfo(NLog.LogLevel.Error, "Twilio", null, string.Format("Validation failed for Account: {0} From: {1} Msg: {2}", incomingMessage.AccountSid, incomingMessage.From, incomingMessage.Body), null, null);
                errorReader.WriteLogMessage(errorEvent);
                return TwiML(messagingResponse);
            }
            /*LogReader logReader = new LogReader();
            var logEvent = new LogEventInfo(NLog.LogLevel.Info, "Twilio", null, "Received response from Twilio" + incomingMessage.From, null, null);
            logReader.WriteLogMessage(logEvent);*/
            SchedulerReader reader = new SchedulerReader();
            var notFoundList = reader.UpdateTextResponseFromTwilio(incomingMessage.From, incomingMessage.Body);
            return TwiML(messagingResponse);
        }
    }
}