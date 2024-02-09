using Intervent.Web.DataLayer;
using NLog;

namespace Intervent.Business
{
    public class AdminManager : BaseManager
    {
        public int AssignNewsletter()
        {
            int count = 0;
            try
            {
                NewsLetterReader reader = new NewsLetterReader();
                count = reader.AddNewsletterToUserDashbaord();
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
            return count;
        }
    }
}
