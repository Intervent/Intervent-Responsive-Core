namespace Intervent.Web.DTO
{
    public class GetAllHRAsforUserRequest
    {
        public int UserId { get; set; }

        public bool IncludeInactivePortalHRAs { get; set; }
    }
}
