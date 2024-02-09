namespace Intervent.Web.DTO
{
    public class AppointmentCallTemplateDto
    {
        public int? Id { get; set; }

        public int NoOfCalls { get; set; }

        public int NoOfWeeks { get; set; }

        public string TemplateName { get; set; }

        public System.DateTime UpdatedDate { get; set; }

        public int UpdatedBy { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<AppointmentCallIntervalDto> CallIntervals { get; set; }
    }

    public partial class AppointmentCallIntervalDto
    {
        public int? Id { get; set; }
        public int ApptCallTemplateId { get; set; }
        public int CallNumber { get; set; }
        public int IntervalInDays { get; set; }

    }
}
