using NLog;

namespace Intervent.Business
{
    public abstract class BaseManager
    {
        protected Logger Log { get; private set; }

        public BaseManager()
        {
            Log = LogManager.GetLogger(GetType().FullName);
        }
    }
}
