namespace Intervent.Web.DTO
{
    public class CarePlanReportDto
    {
        public int Id { get; set; }

        public int? Type { get; set; }

        public string RefId { get; set; }

        public bool? ReportGenerated { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public virtual UserDto User { get; set; }
    }
}
