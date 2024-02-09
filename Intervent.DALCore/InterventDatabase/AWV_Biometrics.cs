namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_Biometrics
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public float? Weight { get; set; }

        public float? Height { get; set; }

        public float? Waist { get; set; }

        public float? LDL { get; set; }

        public float? HDL { get; set; }

        public short? SBP { get; set; }

        public short? DBP { get; set; }

        public float? TotalChol { get; set; }

        public float? Glucose { get; set; }

        public float? A1C { get; set; }

        public float? Trig { get; set; }

        public byte? Fasting { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BPDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? LDLDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? HDLDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? A1CDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? GlucoseDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? TotalCholDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? TrigDate { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
