namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserDrugAllergy")]
    public partial class UserDrugAllergy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserDrugId { get; set; }

        public int AllergyId { get; set; }

        public string? Notes { get; set; }

        public virtual Drug_Allergy Drug_Allergy { get; set; }

        public virtual UserDrug UserDrug { get; set; }
    }
}
