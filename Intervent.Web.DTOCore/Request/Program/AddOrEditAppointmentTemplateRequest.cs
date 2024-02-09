namespace Intervent.Web.DTO
{
    public class AddOrEditAppointmentTemplateRequest
    {
        public AppointmentCallTemplateDto Template { get; set; }
        public int? templateid { get; set; }
    }

    public class ListAppointmentTemplateRequest
    {
        public string AppointmentTemplateId { get; set; }
    }

    public class AddOrEditAppointmentIntervalRequest
    {
        public string AppointmentTemplateId { get; set; }
        public int? templateid { get; set; }
    }
}
