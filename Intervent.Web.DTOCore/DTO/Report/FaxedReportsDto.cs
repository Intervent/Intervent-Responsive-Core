namespace Intervent.Web.DTO
{
    public class FaxedReportsDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int SentBy { get; set; }

        public DateTime? SentOn { get; set; }

        public string FaxNumber { get; set; }

        public string RefId { get; set; }

        public int? ReportType { get; set; }

        public bool? IsSent { get; set; }

        public virtual UserDto User { get; set; }

        public virtual UserDto User1 { get; set; }

        public virtual ReportTypeDto ReportType1 { get; set; }
    }
}
