using Intervent.Web.DataLayer;

namespace Intervent.Business.Tasks
{
    public class TaskManager : BaseManager
    {
        public void CreateTaskforMissedAppt()
        {
            AdminReader reader = new AdminReader();
            reader.CreateTaskforMissedAppt(SystemAdminId);
        }

    }
}
