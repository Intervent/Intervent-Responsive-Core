namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("IntuityQOH")]
    public partial class IntuityQOH
    {
        public int Id { get; set; }

        public int IntuityEligibilityId { get; set; }

        public int QuantityOnHand { get; set; }

        public bool? Submitted { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual IntuityEligibility IntuityEligibility { get; set; }
    }
}
