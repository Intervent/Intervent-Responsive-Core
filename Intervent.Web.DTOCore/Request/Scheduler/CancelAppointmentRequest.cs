namespace Intervent.Web.DTO
{
    public class CancelAppointmentRequest
    {
        public int id { get; set; }

        public byte reason { get; set; }

        public string comments { get; set; }

        public int updatedBy { get; set; }

    }
}
