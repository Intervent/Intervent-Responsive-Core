using Intervent.DAL;

namespace Intervent.Web.DTO
{
    public class GetIntuityUsersResponse
    {
        public IntuityUsers IntuityUsers { get; set; }

        public bool IsNewUser { get; set; }
    }
}
