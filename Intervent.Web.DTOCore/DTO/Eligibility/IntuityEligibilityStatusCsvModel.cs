using System.ComponentModel.DataAnnotations;

namespace Intervent.Web.DTO
{
    public class IntuityEligibilityStatusCsvModel
    {

        [MaxLength(100)]
        public string PatientUniqueId { get; set; }

        public string BenefitHolderId { get; set; }

        public DateTime? Dob { get; set; }

        [MaxLength(128)]
        public string Firstname { get; set; }

        public string Middlename { get; set; }

        [MaxLength(128)]
        public string Lastname { get; set; }

        [MaxLength(256)]
        public string EmailAddress { get; set; }

        public GenderDto Gender { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(15)]
        public string Zip { get; set; }

        public string Phone { get; set; }

        public string UserEnrollmentType { get; set; }

        public DateTime? HireDate { get; set; }

        public DateTime? TerminationDate { get; set; }

        public string UserStatus { get; set; }

        [MaxLength(100)]
        public string BusinessUnit { get; set; }

        [MaxLength(100)]
        public string RegionCode { get; set; }

        public string UnionFlag { get; set; }

        public string PayType { get; set; }

        [MaxLength(50)]
        public string MedicalPlanCode { get; set; }

        public DateTime? MedicalPlanStartDate { get; set; }

        public DateTime? MedicalPlanEndDate { get; set; }

        public string TobaccoFlag { get; set; }


    }
}
