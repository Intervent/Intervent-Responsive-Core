namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class UserDashboardMessage
    {
        public int Id { get; set; }

        [StringLength(512)]
        public string? Message { get; set; }

        [StringLength(100)]
        public string? Url { get; set; }

        public bool New { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedOn { get; set; }

        public int MessageType { get; set; }

        public int? RelatedId { get; set; }

        public bool Active { get; set; }

        [StringLength(255)]
        public string? Parameters { get; set; }

        public byte Status { get; set; }

        public virtual DashboardMessageType DashboardMessageType { get; set; }

        public virtual User User { get; set; }
    }
}
