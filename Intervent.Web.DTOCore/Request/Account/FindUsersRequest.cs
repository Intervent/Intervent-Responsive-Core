namespace Intervent.Web.DTO
{
    public class FindUsersRequest
    {
        public IEnumerable<int> OrganizationIds { get; set; }

        public int? UserId { get; set; }

    }
}
