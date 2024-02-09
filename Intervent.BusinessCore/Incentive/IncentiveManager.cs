using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;

namespace Intervent.Business
{
    public class IncentiveManager : BaseManager
    {
        public ProcessIncentives_ResultDto ProcessIncentives()
        {
            ProcessIncentives_ResultDto response = new ProcessIncentives_ResultDto();
            try
            {
                IncentiveReader reader = new IncentiveReader();
                response = reader.ProcessIncentives();
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
            return response;
        }

        public int ProcessUserKeys()
        {
            int userKeysCount = 0;
            try
            {
                IncentiveReader reader = new IncentiveReader();
                userKeysCount = reader.ProcessUserKeys();
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
            return userKeysCount;
        }
    }
}
