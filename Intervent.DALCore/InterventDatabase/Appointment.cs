namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class Appointment
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public int? Type { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; }

        public bool Active { get; set; }

        public int? InActiveReason { get; set; }

        public string? InActiveComment { get; set; }

        public int CoachId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public byte Minutes { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? NSHandledBy { get; set; }

        [StringLength(150)]
        public string? MessageSID { get; set; }

        public bool VideoRequired { get; set; }

        public virtual AppointmentType AppointmentType { get; set; }

        public virtual AppointmentFeedback AppointmentFeedback { get; set; }

        public virtual CancellationReason CancellationReason { get; set; }

        public string? TextResponse { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual User User2 { get; set; }

        public virtual User User3 { get; set; }

        public virtual User User4 { get; set; }
    }
}
