namespace Intervent.Web.DTO
{
    public class UserIncentiveDto
    {
        public DateTime DateCreated { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public virtual PortalIncentiveDto PortalIncentive { get; set; }
        public int PortalIncentiveId { get; set; }
        public int UserId { get; set; }
        public string Reference { get; set; }
        public double Points { get; set; }
        public string Comments { get; set; }
    }
}