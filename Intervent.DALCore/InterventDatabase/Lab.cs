namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Lab
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lab()
        {
            LabDatas = new HashSet<LabData>();
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        public int PortalId { get; set; }

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

        [StringLength(256)]
        public string? LabOrder { get; set; }

        public byte? LabSelection { get; set; }

        public DateTime? DateCompleted { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        [StringLength(50)]
        public string? OrderNo { get; set; }

        public string? HL7 { get; set; }

        public DateTime? ReviewedOn { get; set; }

        [StringLength(30)]
        public string? CoachAlert { get; set; }

        public int? ReviewedBy { get; set; }

        public byte? SwitchCount { get; set; }

        public byte? ReminderEmailSent { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        [StringLength(30)]
        public string? CriticalAlert { get; set; }

        public bool? HighCotinine { get; set; }

        public int? DiagnosticLabId { get; set; }

        public byte? RejectionReason { get; set; }

        public DateTime? RejectedOn { get; set; }

        public int? RejectedBy { get; set; }

        public bool AdditionalLab { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LabData> LabDatas { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual Portal Portal { get; set; }

        public virtual User User2 { get; set; }

        public virtual User User3 { get; set; }

        public virtual DiagnosticLab DiagnosticLab { get; set; }

    }
}
