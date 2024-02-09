namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_Allergies
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(100)]
        public string? DrugName { get; set; }

        [StringLength(255)]
        public string? Allergy { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
