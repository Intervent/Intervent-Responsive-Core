namespace Intervent.Web.DTO
{
    public class InsuranceSummaryDto
    {
        public int ID { get; set; }

        public DateTime? EarliestServiceDate { get; set; }

        public bool Eligible { get; set; }

        public bool HRA { get; set; }

        public string EnrollType { get; set; }

        public string DataSrc { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int ConditionCount { get; set; }

        public bool Inactive { get; set; }

        public string UniqueID { get; set; }

        public int? OrganizationId { get; set; }

        public bool Spouse { get; set; }

        public virtual ICollection<CandidateConditionsDto> CandidateConditions { get; set; }

        public virtual ICollection<CandidateMedicationsDto> CandidateMedications { get; set; }

        public virtual ICollection<ClaimConditionCodeDto> ClaimConditionCodes { get; set; }

        public virtual ICollection<CandidateReasonForLastChangeDto> CandidateReasonForLastChanges { get; set; }
    }

}
