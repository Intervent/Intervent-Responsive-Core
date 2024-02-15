namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CRM_Notes")]
    public partial class CRM_Note
    {
        public int Id { get; set; }

        public int ContactId { get; set; }

        public byte? Type { get; set; }

        public int? CallerProfileType { get; set; }

        [StringLength(50)]
        public string? FullName { get; set; }

        public int? AccountType { get; set; }

        public byte? RequiredEscalation { get; set; }

        public byte? RequiredRMA { get; set; }

        [StringLength(50)]
        public string? RMANumber { get; set; }

        [StringLength(50)]
        public string? QADNumber { get; set; }

        public int? ComplaintClassificationType { get; set; }

        public byte? FirstCallResolution { get; set; }

        public byte? POGOChampion { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public Int64? CallId { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string? Ani { get; set; }

        public string? Dnis { get; set; }

        public int? CampaignType { get; set; }

        [StringLength(50)]
        public string? Campaign { get; set; }

        public byte? Calls { get; set; }

        [StringLength(50)]
        public string? CallType { get; set; }

        public TimeSpan? SpeedofAnswer { get; set; }

        public TimeSpan? QueueWaitTime { get; set; }

        public TimeSpan? CallTime { get; set; }

        public int? Disposition { get; set; }

        public byte? Holds { get; set; }

        public TimeSpan? HoldTime { get; set; }

        public TimeSpan? AfterCallWorkTime { get; set; }

        public TimeSpan? HandleTime { get; set; }

        public byte? Abandoned { get; set; }

        public float? AbandonRate { get; set; }

        public short? Conferences { get; set; }

        public short? Transfers { get; set; }

        public TimeSpan? QueueCallBackWaitTime { get; set; }

        public byte? Survey1 { get; set; }

        public byte? Survey2 { get; set; }

        public byte? Survey3 { get; set; }

        public byte? Survey4 { get; set; }

        public TimeSpan? TalkTime { get; set; }

        public byte? Voicemails { get; set; }

        [StringLength(50)]
        public string? MasterControlNo { get; set; }

        [StringLength(50)]
        public string? CartridgeLotNumber { get; set; }

        public TimeSpan? VoicemailsHandleTime { get; set; }

        public int? PogoMeterNumber { get; set; }

        public int? HCPId { get; set; }

        [StringLength(50)]
        public string? CouponCode { get; set; }

        [StringLength(250)]
        public string? CartridgePrescriptions { get; set; }

        public int? InquiryType { get; set; }

        [StringLength(10)]
        public string Language { get; set; }

        public virtual CRM_AccountType CRM_AccountTypes { get; set; }

        public virtual CRM_CallerProfileType CRM_CallerProfileTypes { get; set; }

        public virtual CRM_ComplaintClassificationType CRM_ComplaintClassificationTypes { get; set; }

        public virtual CRM_Disposition CRM_Dispositions { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual CRM_Contact CRM_Contacts { get; set; }

        public virtual CRM_PogoMeterNumbers CRM_PogoMeterNumbers { get; set; }

        public virtual HCPList HCPList { get; set; }

        public virtual InquiryType InquiryType1 { get; set; }

    }
}
