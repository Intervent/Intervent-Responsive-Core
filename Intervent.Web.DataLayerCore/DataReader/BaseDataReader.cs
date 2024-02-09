using Intervent.DAL;

namespace Intervent.Web.DataLayer
{
    public abstract class BaseDataReader
    {
        public BaseDataReader()
        {
            Context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());
        }

        protected InterventDatabase Context { get; private set; }

        //need to get rid of the constructor context since it continuously builds the context without disposing it
        protected InterventDatabase GetContext
        {
            get
            {
                return new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());
            }
        }
    }
}
