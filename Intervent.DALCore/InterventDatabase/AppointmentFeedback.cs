namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class AppointmentFeedback
    {
        [Key]
        public int AppointmentId { get; set; }

        public int? Rating { get; set; }

        public string? Comments { get; set; }

        public virtual Appointment Appointment { get; set; }
    }
}
