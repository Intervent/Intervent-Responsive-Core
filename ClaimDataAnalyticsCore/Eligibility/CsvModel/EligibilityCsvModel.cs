using ClaimDataAnalytics.Eligibility.CodeDto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClaimDataAnalytics.Eligibility.CsvModel
{
    public sealed class EligibilityCsvModel
    {
        [Required]
        [MaxLength(100)]
        public string UniqueId { get; set; }

        public string SSN { get; set; }
        //public string NamePrefix { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

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
        [RegularExpression("^[0-9]{5}$", ErrorMessage = "Invalid ZIP")]
        public string ZipOrPostalCode { get; set; }

        [MaxLength(50)]
        // [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
        public string EmailAddress { get; set; }

        //[RegularExpression("^[1-9][0-9]{9}$")]
        public string HomePhone { get; set; }

        public EligibilityUserEnrollmentTypeDto UserEnrollmentType { get; set; }

        [MaxLength(100)]
        public string EmployeeUniqueId { get; set; }

        public DateTime? HireDate { get; set; }

        public DateTime? TerminatedDate { get; set; }

        public DateTime? DeathDate { get; set; }

        [MaxLength(100)]
        public string BusinessUnit { get; set; }

        [MaxLength(100)]
        // [Required]
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

        public string CompanyName { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
