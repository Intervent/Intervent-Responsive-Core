namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("IntuityFulfillment")]
    public partial class IntuityFulfillments
    {
        public int Id { get; set; }

        public int IntuityEligibilityId { get; set; }

        public string? TrackingNumber { get; set; }

        [StringLength(512)]
        public string? SerialNumber { get; set; }

        public byte RefillRequested { get; set; }

        public DateTime? RefillRequestDate { get; set; }

        public byte? RefillSent { get; set; }

        public bool? SendMeter { get; set; }

        public DateTime? RefillSentDate { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string? SoNbr { get; set; }

        public virtual IntuityEligibility IntuityEligibility { get; set; }
    }
}
