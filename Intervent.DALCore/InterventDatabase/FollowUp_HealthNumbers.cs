namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class FollowUp_HealthNumbers
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? BPArm { get; set; }

        public short? SBP { get; set; }

        public short? DBP { get; set; }

        public byte? DidYouFast { get; set; }

        public DateTime? BloodTestDate { get; set; }

        public float? TotalChol { get; set; }

        public float? LDL { get; set; }

        public float? HDL { get; set; }

        public float? Trig { get; set; }

        public float? Glucose { get; set; }

        public float? A1C { get; set; }

        public float? Weight { get; set; }

        public float? Height { get; set; }

        public float? Waist { get; set; }

        public short? RMR { get; set; }

        public short? THRFrom { get; set; }

        public short? THRTo { get; set; }

        public float? BMI { get; set; }

        public float? CRF { get; set; }

        public float? RHR { get; set; }

        public int? LabId { get; set; }

        public virtual FollowUp FollowUp { get; set; }

        public virtual Lab Lab { get; set; }
    }
}
