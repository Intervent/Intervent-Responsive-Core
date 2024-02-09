namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_Medications
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? Dosage { get; set; }

        [StringLength(50)]
        public string? Frequency { get; set; }

        [StringLength(64)]
        public string? Code { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
