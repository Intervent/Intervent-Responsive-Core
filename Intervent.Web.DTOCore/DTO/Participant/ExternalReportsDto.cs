namespace Intervent.Web.DTO
{
    public class ExternalReportsDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ReportName { get; set; }

        public byte[] ReportData { get; set; }

        public DateTime? CreatedOn { get; set; }

        public UserDto User { get; set; }
    }
}
