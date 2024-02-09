namespace Intervent.Web.DTO
{
    public class EligibilityDto
    {
        public int? Id { get; set; }

        public int PortalId { get; set; }

        public string UniqueId { get; set; }

        public string NamePrefix { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public Nullable<System.DateTime> DOB { get; set; }

        public GenderDto Gender { get; set; }

        public string Email { get; set; }

        public string HomeNumber { get; set; }

        public string WorkNumber { get; set; }

        public string CellNumber { get; set; }

        public string Address { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Zip { get; set; }

        public string SSN { get; set; }

        public EligibilityUserEnrollmentTypeDto UserEnrollmentType { get; set; }

        public string EmployeeUniqueId { get; set; }

        public DateTime? HireDate { get; set; }

        public DateTime? TerminatedDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public string BusinessUnit { get; set; }

        public string RegionCode { get; set; }

        public bool? UnionFlag { get; set; }

        public string MedicalPlanCode { get; set; }

        public DateTime? MedicalPlanStartDate { get; set; }

        public DateTime? MedicalPlanEndDate { get; set; }

        public bool? TobaccoFlag { get; set; }

        public bool? EducationalAssociates { get; set; }

        public string PayrollArea { get; set; }

        public EligibilityPayTypeDto PayType { get; set; }

        public EligibilityUserStatusDto UserStatus { get; set; }

        public PortalDto Portal { get; set; }

        public byte? EnrollmentStatus { get; set; }

        public byte? DeclinedEnrollmentReason { get; set; }

        public bool? IsFalseReferral { get; set; }

        public string DentalPlanCode { get; set; }

        public DateTime? DentalPlanStartDate { get; set; }

        public DateTime? DentalPlanEndDate { get; set; }

        public string VisionPlanCode { get; set; }

        public DateTime? VisionPlanStartDate { get; set; }

        public DateTime? VisionPlanEndDate { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedDate { get; set; }

        public string Ref_LastName { get; set; }

        public string Ref_FirstName { get; set; }

        public string Ref_PractNum { get; set; }

        public string Ref_OfficeName { get; set; }

        public string Ref_City { get; set; }

        public string Ref_StateOrProvince { get; set; }

        public string Ref_Phone { get; set; }

        public string Ref_Fax { get; set; }

        public DateTime? CreateDate { get; set; }

        public EligibilityUserDiabetesTypeDto DiabetesType { get; set; }

        public bool? CoachingEnabled { get; set; }

        public DateTime? CoachingExpirationDate { get; set; }

        public string Email2 { get; set; }

        public DateTime? Lab_Date { get; set; }

        public byte? Lab_DidYouFast { get; set; }

        public float? Lab_A1C { get; set; }

        public float? Lab_Glucose { get; set; }

        public DateTime? FirstEligibleDate { get; set; }

        public string Ref_Address1 { get; set; }

        public string Ref_Address2 { get; set; }

        public string Ref_Zip { get; set; }

        public string Ref_Country { get; set; }

        public string Ref_Phone2 { get; set; }

        public string Ref_Email { get; set; }

        public string Race { get; set; }

        public string TerminationReason { get; set; }
    }

    public class EligibilityPropertyChange
    {
        public string PropertyName { get; set; }

        public string CurrentValue { get; set; }

        public string NewValue { get; set; }
    }
}
