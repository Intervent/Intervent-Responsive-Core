using System.ComponentModel.DataAnnotations;

namespace Intervent.Web.DTO
{
    public sealed class EligibilityCsvModel
    {
        [Required]
        [MaxLength(100)]
        public string UniqueId { get; set; }

        public string SSN { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string MiddleName { get; set; }

        public DateTime? DOB { get; set; }

        public GenderDto Gender { get; set; }

        [MaxLength(130)]
        public string Address1 { get; set; }

        [MaxLength(125)]
        public string Address2 { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string StateOrProvince { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(15)]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Invalid ZIP")]
        public string ZipOrPostalCode { get; set; }

        [MaxLength(50)]
        public string EmailAddress { get; set; }

        public string HomePhone { get; set; }

        public string Phone { get; set; }

        public string CellNumber { get; set; }

        public EligibilityUserEnrollmentTypeDto UserEnrollmentType { get; set; }

        [MaxLength(100)]
        public string EmployeeUniqueId { get; set; }

        public DateTime? HireDate { get; set; }

        public DateTime? TerminatedDate { get; set; }

        public DateTime? DeathDate { get; set; }

        [MaxLength(100)]
        public string BusinessUnit { get; set; }

        [MaxLength(100)]
        public string RegionCode { get; set; }

        public YesNoDto UnionFlag { get; set; }

        [MaxLength(50)]
        public string MedicalPlanCode { get; set; }

        public DateTime? MedicalPlanStartDate { get; set; }

        public DateTime? MedicalPlanEndDate { get; set; }

        public YesNoDto TobaccoFlag { get; set; }

        public YesNoDto EducationalAssociates { get; set; }

        public EligibilityPayTypeDto PayType { get; set; }

        public EligibilityUserStatusDto UserStatus { get; set; }

        [MaxLength(50)]
        public string DentalPlanCode { get; set; }

        public DateTime? DentalPlanStartDate { get; set; }

        public DateTime? DentalPlanEndDate { get; set; }

        [MaxLength(50)]
        public string VisionPlanCode { get; set; }

        [MaxLength(2)]
        public string PayrollArea { get; set; }

        public DateTime? VisionPlanStartDate { get; set; }

        public DateTime? VisionPlanEndDate { get; set; }

        public string Ref_LastName { get; set; }

        public string Ref_FirstName { get; set; }

        public string Ref_PractNum { get; set; }

        public string Ref_OfficeName { get; set; }

        public string Ref_City { get; set; }

        public string Ref_Province { get; set; }

        public string Ref_OfficePhone { get; set; }

        public string Ref_FaxPhone { get; set; }

        public string CompanyCode { get; set; }

        public EligibilityUserDiabetesTypeDto DiabetesType { get; set; }

        public TrueFalseDto CoachingEnabled { get; set; }

        public DateTime? CoachingExpirationDate { get; set; }

        [MaxLength(50)]
        public string PatternsEmailID { get; set; }

        [MaxLength(20)]
        public string PatternsPhoneNumber { get; set; }

        public DateTime? Lab_Date { get; set; }

        public DidYouFastDto Lab_Fasting { get; set; }

        public Decimal? Lab_A1C { get; set; }

        public Decimal? Lab_FBS { get; set; }

        public string Ref_Address1 { get; set; }

        public string Ref_Address2 { get; set; }

        public string Ref_Zip { get; set; }

        public string Ref_Country { get; set; }

        public string Ref_Phone2 { get; set; }

        public string Ref_Email { get; set; }

        public string Race { get; set; }

        public string TerminationReason { get; set; }

    }
    public class GlucometerCsvModel
    {
        public string client_uid { get; set; }

        public DateTime? registeredDate { get; set; }
        public DateTime? activationDate { get; set; }
    }
}
