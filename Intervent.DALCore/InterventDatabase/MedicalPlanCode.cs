namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class MedicalPlanCode
    {
        [StringLength(50)]
        public string? Code { get; set; }

        [Key]
        [Column(Order = 0)]
        public bool IVEligible { get; set; }

        [Key]
        [Column(Order = 1)]
        public bool VendorEligible { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }
    }
}
