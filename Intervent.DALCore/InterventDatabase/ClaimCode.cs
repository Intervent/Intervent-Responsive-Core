using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intervent.DAL
{
    [Table("IVClaimsCodes")]
    public class ClaimCode
    {
        [Key]
        public int IvcRef { get; set; }

        public string Code { get; set; }

        public string CodeFlag { get; set; }

        public string CodeSource { get; set; }

        public string CodeDescription { get; set; }

        public bool IncludeME { get; set; }
    }

    [Table("TherapeuticClassCodes")]
    public class TherapeuticClassCode
    {
        [Key]
        public string TheraCode { get; set; }

        public string DrugCategory { get; set; }
    }

    [Table("LivongoICDCodes")]
    public class LivongoICDCodes
    {
        [Key]
        public int LvRef { get; set; }

        public string LvCode { get; set; }

        public string LvCodeFlag { get; set; }

        public string LvCodeSource { get; set; }

        public string LvCodeDescription { get; set; }

        public bool IsActive { get; set; }
    }

    [Table("LivongoNDCCodes")]
    public class LivongoNDCCodes
    {
        [Key]
        public int LvNDCRef { get; set; }

        public string LvNDCCode { get; set; }

        public bool IsActive { get; set; }

    }

    [Table("ClaimConditionCode")]
    public class ClaimConditionCode
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public int? ClaimsID { get; set; }

        public string? Condition { get; set; }

        public DateTime? ConditionDate { get; set; }

        [StringLength(15)]
        public string? Code { get; set; }

        public string? CodeDescription { get; set; }

        public virtual InsuranceSummary InsuranceSummary { get; set; }
    }
}
