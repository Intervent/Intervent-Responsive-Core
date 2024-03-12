namespace Intervent.Web.DTO
{
    public class CRM_NoteDto
    {
        public int Id { get; set; }

        public int ContactId { get; set; }

        public byte? Type { get; set; }

        public byte? InsuranceType { get; set; }

        public int? CallerProfileType { get; set; }

        public string FullName { get; set; }

        public int? AccountType { get; set; }

        public byte? RequiredEscalation { get; set; }

        public byte? RequiredRMA { get; set; }

        public string RMANumber { get; set; }

        public string QADNumber { get; set; }

        public int? ComplaintClassificationType { get; set; }

        public int? PogoMeterNumber { get; set; }

        public byte? FirstCallResolution { get; set; }

        public byte? POGOChampion { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedOnDateText { get; set; }

        public string CreatedOnTimeText { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public Int64? CallId { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string Ani { get; set; }

        public string Dnis { get; set; }

        public int? CampaignType { get; set; }

        public string Campaign { get; set; }

        public byte? Calls { get; set; }

        public string CallType { get; set; }

        public TimeSpan? SpeedofAnswer { get; set; }

        public TimeSpan? QueueWaitTime { get; set; }

        public TimeSpan? CallTime { get; set; }

        public string DispositionPath { get; set; }

        public int? Disposition { get; set; }

        public bool EligibleforActivity { get; set; }

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

        public TimeSpan? VoicemailsHandleTime { get; set; }

        public string MasterControlNo { get; set; }

        public string CartridgeLotNumber { get; set; }

        public int? HCPId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public int State { get; set; }

        public string Zipcode { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string CouponCode { get; set; }

        public string CartridgePrescriptions { get; set; }

        public int? InquiryType { get; set; }

        public string Language { get; set; }

        public int? InsType { get; set; }

        public virtual CRM_AccountTypesDto CRM_AccountTypes { get; set; }

        public virtual CRM_CallerProfileTypesDto CRM_CallerProfileTypes { get; set; }

        public virtual CRM_ComplaintClassificationTypesDto CRM_ComplaintClassificationTypes { get; set; }

        public virtual CRM_ContactDto CRM_Contacts { get; set; }

        public virtual UserDto User { get; set; }

        public virtual UserDto User1 { get; set; }

        public virtual CRM_DispositionsDto CRM_Dispositions { get; set; }

        public virtual CRM_PogoMeterNumbersDto CRM_PogoMeterNumbers { get; set; }

        public virtual HCPListsDto HCPList { get; set; }

        public virtual InquiryTypeDto InquiryType1 { get; set; }

    }
}
