using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class EligibilityUserModel
    {
        public string Address { get; set; }

        public string Address2 { get; set; }

        public string BusinessUnit { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public DateTime? DeathDate { get; set; }

        public DateTime? DOB { get; set; }

        public string EligibilityStatus { get; set; }

        public string Email { get; set; }

        public string EmployeeUniqueId { get; set; }

        public string FirstName { get; set; }

        public string Gender { get; set; }

        public DateTime? HireDate { get; set; }

        public string HomeNumber { get; set; }

        public string CellNumber { get; set; }

        public int Id { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string MedicalPlanCode { get; set; }

        public DateTime? MedicalPlanEndDate { get; set; }

        public DateTime? MedicalPlanStartDate { get; set; }

        public string DentalPlanCode { get; set; }

        public DateTime? DentalPlanEndDate { get; set; }

        public DateTime? DentalPlanStartDate { get; set; }

        public string VisionPlanCode { get; set; }

        public DateTime? VisionPlanEndDate { get; set; }

        public DateTime? VisionPlanStartDate { get; set; }

        public string PayType { get; set; }

        public string PayrollArea { get; set; }

        public string RegionCode { get; set; }

        public string SSN { get; set; }

        public string State { get; set; }

        public DateTime? TerminatedDate { get; set; }

        public string TobaccoFlag { get; set; }

        public string EducationalAssociates { get; set; }

        public string UnionFlag { get; set; }

        public string UniqueId { get; set; }

        public string UserEnrollmentType { get; set; }

        public string WorkNumber { get; set; }

        public string Zip { get; set; }

        public int? PortalUserId { get; set; }

        public int MissedYouEmail { get; set; }

        public float? Hracompletion { get; set; }

        public string ProgramStatus { get; set; }

        public int? PrimaryUserId { get; set; }

        public DateTime? PrimaryUserDOB { get; set; }

        public string PrimaryUserEmail { get; set; }

        public string PrimaryUserFirstName { get; set; }

        public string PrimaryUserGender { get; set; }

        public string PrimaryUserLastName { get; set; }

        public int SpouseId { get; set; }

        public string SpouseFirstName { get; set; }

        public string SpouseLastName { get; set; }

        public string SpouseGender { get; set; }

        public DateTime? SpouseDOB { get; set; }

        public bool CanAddSpouseEligibility { get; set; }

        public string medicalCodeComment { get; set; }

        public string Location { get; set; }

        public int? EnrollmentStatus { get; set; }

        public int? DeclinedEnrollmentReason { get; set; }

        public bool? IsFalseReferral { get; set; }

        public string PortalId { get; set; }

        public int OrganizationId { get; set; }

        public byte? IntegrationWith { get; set; }

        public bool IsIntuityDTC { get; set; }

        public string Ref_LastName { get; set; }

        public string Ref_FirstName { get; set; }

        public string Ref_PractNum { get; set; }

        public string Ref_OfficeName { get; set; }

        public string Ref_City { get; set; }

        public string Ref_StateOrProvince { get; set; }

        public string Ref_Phone { get; set; }

        public string Ref_Fax { get; set; }

        public DateTime? Lab_Date { get; set; }

        public string Lab_DidYouFast { get; set; }

        public float? Lab_A1C { get; set; }

        public float? Lab_Glucose { get; set; }

        public string DiabetesType { get; set; }

        public bool? CoachingEnabled { get; set; }

        public DateTime? CoachingExpirationDate { get; set; }

        public string Email2 { get; set; }

        public byte EligibilityFormat { get; set; }

        public CanriskModel canriskModel { get; set; }

        public DateTime? FirstEligibleDate { get; set; }

        public string Ref_Address1 { get; set; }

        public string Ref_Address2 { get; set; }

        public string Ref_Zip { get; set; }

        public string Ref_Country { get; set; }

        public string Ref_Phone2 { get; set; }

        public string Ref_Email { get; set; }

        public string Race { get; set; }

        public string TerminationReason { get; set; }

        public int? CRMId { get; set; }

        public bool fromElgPage { get; set; }

        public string dateFormat { get; set; }
    }

    public class EligibilityNotesModel
    {
        public EligibilityNotesDto eligibilityNotes { get; set; }

    }

    public class UpdateEligiblityModel
    {
        public int Id { get; set; }

        public bool falseReferral { get; set; }

        public string language { get; set; }

        public byte? enrollmentStatus { get; set; }

        public byte? declinedEnrollmentReason { get; set; }

        public bool isSecEmail { get; set; }

        public string email2 { get; set; }

    }


    public class IntuityEligibilityModel
    {
        public bool IsUserExists { get; set; }

        public string UniqueId { get; set; }

        public string OrganizationCode { get; set; }

        public int OrganizationId { get; set; }

        public int PortalId { get; set; }

        public string diabetesDate { get; set; }

        public string a1cTestDate { get; set; }

        public bool OverrideStatus { get; set; }

        public bool ShowNewForm { get; set; }

        public bool ShowFulfillment { get; set; }

        public bool UserEligible { get; set; }

        public int IntuityEligibilityId { get; set; }

        public float? HeightInch { get; set; }

        public float? HeightFeet { get; set; }

        public int UpdatedBy { get; set; }

        public string DateFormat { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }

        public IntuityEligibilityLogDto IntuityEligibilityLog { get; set; }

        public IEnumerable<SelectListItem> DiabetesTypes { get; set; }

        public IEnumerable<SelectListItem> EligibilityStatusList { get; set; }

        public IEnumerable<SelectListItem> EligibilityReasonsList { get; set; }

        public IEnumerable<SelectListItem> NoA1cTestReason { get; set; }

        public IList<IntuityFulfillmentsDto> IntuityFulfillments { get; set; }

        public IList<IntuityQOHDto> IntuityQOH { get; set; }

        public string IntuityFulfillmentRequests { get; set; }

        public string ImmediateShipmentRequests { get; set; }

        public IntuityEligibilityDto IntuityEligibility { get; set; }

        public string EligibilityStatus { get; set; }

        public DateTime? FormSubmittedDate { get; set; }

        public DateTime? PatternsRegDate { get; set; }

    }
}