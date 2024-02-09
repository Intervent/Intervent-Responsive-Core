using NLog;
using System.Configuration;

namespace Intervent.Business
{
    public abstract class BaseManager
    {
        protected Logger Log { get; private set; }

        public static int SystemAdminId { get; set; }

        public BaseManager()
        {
            Log = LogManager.GetLogger(GetType().FullName);
            SystemAdminId = int.Parse(ConfigurationManager.AppSettings["SystemAdminId"]);
        }
    }
}
