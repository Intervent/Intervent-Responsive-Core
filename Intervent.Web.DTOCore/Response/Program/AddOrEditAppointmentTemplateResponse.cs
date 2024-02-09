namespace Intervent.Web.DTO
{
    public class AddOrEditAppointmentTemplateResponse
    {
        public AppointmentCallTemplateDto CallTemplate { get; set; }
        public IEnumerable<AppointmentCallIntervalDto> CallIntervals { get; set; }
        public bool? status { get; set; }
    }

    public class ListAppointmentTemplateResponse
    {
        public IEnumerable<AppointmentCallTemplateDto> CallTemplates { get; set; }
    }

    public class AddOrEditAppointmentIntervalResponse
    {
        public IEnumerable<AppointmentCallIntervalDto> CallIntervals { get; set; }
    }

}
