namespace Intervent.DAL
{
    using System;

    public partial class Availability
    {
        public int Id { get; set; }

        public int CoachId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public long RefId { get; set; }

        public bool Active { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool IsBooked { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual User User2 { get; set; }
    }
}
