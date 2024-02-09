using Intervent.DAL;

namespace Intervent.Web.DTO
{
    public class GetIntuityUserResponse
    {
        public List<IntuityUserWrapper> UserList { get; set; }

        public List<EligibilityDto> NewUsers { get; set; }
    }

    public class IntuityUserWrapper
    {
        public DAL.IntuityUsers IntUser { get; set; }

        public Eligibility Eligibility { get; set; }

    }
}
