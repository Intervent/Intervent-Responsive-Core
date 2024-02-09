using Intervent.Web.DataLayer;
using NLog;

namespace Intervent.Business.FollowUp
{
    public class RecipeManager : BaseManager
    {
        public int SendRecipe()
        {
            int count = 0;
            try
            {
                RecipeReader reader = new RecipeReader();
                count = reader.AddRecipetoUserDashbaord();
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
