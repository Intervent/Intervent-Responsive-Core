namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class FollowUp_Goals
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public float? StWt { get; set; }

        public float? LtWt { get; set; }

        public float? LtLdl { get; set; }

        public float? LtHdl { get; set; }

        public float? LtTrig { get; set; }

        public short? LtSbp { get; set; }

        public short? LtDbp { get; set; }

        public float? LtGluc1 { get; set; }

        public float? LtGluc2 { get; set; }

        public float? LtA1c { get; set; }

        public bool? ASCVD { get; set; }

        public bool? Diabetes { get; set; }

        public bool? CholRef { get; set; }

        public bool? BPRef { get; set; }

        public bool? LdlRef1 { get; set; }

        public bool? LdlRef2 { get; set; }

        public bool? HdlRef { get; set; }

        public int? ASCVDRef { get; set; }

        public bool? TrigRef1 { get; set; }

        public bool? TrigRef2 { get; set; }

        public bool? DiabRef { get; set; }

        public bool? NicRef { get; set; }

        public bool? AspRef { get; set; }

        public float? TenYrProb { get; set; }

        public float? TenYrAvg { get; set; }

        public float? TenYrLow { get; set; }

        public float? TenYearASCVD { get; set; }

        public float? TenYearASCVDGoal { get; set; }

        public float? LifetimeASCVD { get; set; }

        public float? LifetimeASCVDGoal { get; set; }

        public bool? ElevatedBPRef { get; set; }

        public bool? HypertensiveBPRef { get; set; }

        public virtual FollowUp FollowUp { get; set; }
    }
}
