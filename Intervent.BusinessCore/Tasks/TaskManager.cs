using Intervent.Web.DataLayer;
using System.Configuration;

namespace Intervent.Business.Tasks
{
    public class TaskManager : BaseManager
    {
        public void CreateTaskforMissedAppt()
        {
            AdminReader reader = new AdminReader();
            reader.CreateTaskforMissedAppt(Convert.ToInt32(ConfigurationManager.AppSettings["SystemAdminId"]));
        }

    }
}
