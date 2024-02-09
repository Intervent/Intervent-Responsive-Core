namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class UserDrug
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string MedicationName { get; set; }

        [Required]
        [StringLength(100)]
        public string Dosage { get; set; }

        public byte Quantity { get; set; }

        public int Formulation { get; set; }

        public int Frequency { get; set; }

        public int Condition { get; set; }

        public DateTime MedicationStartDate { get; set; }

        public int Duration { get; set; }

        public string? Notes { get; set; }

        public DateTime AddedOn { get; set; }

        public int AddedBy { get; set; }

        [StringLength(500)]
        public string? Ingredient { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DiscontinuedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual Drug_Condition Drug_Condition { get; set; }

        public virtual Drug_Duration Drug_Duration { get; set; }

        public virtual Drug_Formulation Drug_Formulation { get; set; }

        public virtual Drug_Frequency Drug_Frequency { get; set; }

        public virtual UserDrugAllergy UserDrugAllergy { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual User User2 { get; set; }
    }
}
