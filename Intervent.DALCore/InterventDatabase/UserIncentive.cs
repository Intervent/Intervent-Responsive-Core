namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class UserIncentive
    {
        public int Id { get; set; }

        public int PortalIncentiveId { get; set; }

        public int UserId { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }

        [StringLength(250)]
        public string? Reference { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public double? Points { get; set; }

        public string? Comments { get; set; }

        public virtual PortalIncentive PortalIncentive { get; set; }
    }
}
