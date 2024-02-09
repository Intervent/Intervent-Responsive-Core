namespace Intervent.Web.DTO
{
    public class SmokingCessationIncentiveDto
    {
        public string PortalName { get; set; }
        public int? UserId { get; set; }
        public string UniqueID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ReasonForPass { get; set; }
        public DateTime? DatePassed { get; set; }
        public string InSmokingProgram { get; set; }
    }
}
