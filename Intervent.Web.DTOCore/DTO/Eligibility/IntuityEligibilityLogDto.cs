using System.ComponentModel.DataAnnotations;

namespace Intervent.Web.DTO
{
    public class IntuityEligibilityLogDto
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public DateTime? DOB { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string AddressLine1 { get; set; }

        [Required]
        public string AddressLine2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public int? State { get; set; }

        [Required]
        public int? Country { get; set; }

        [Required]
        public string Zip { get; set; }

        public byte? Gender { get; set; }

        [Required]
        public string UniqueId { get; set; }

        [Required]
        public string OrganizationCode { get; set; }

        public int? OrganizationId { get; set; }

        public int? EligibilityId { get; set; }

        public byte? APIStatus { get; set; }

        public int? HRAId { get; set; }

        public DateTime? DateCreated { get; set; }

        public byte? EligibilityReason { get; set; }

        [Required]
        public byte? TakeDiabetesMed { get; set; }

        public byte? TakeInsulin { get; set; }

        [Required]
        public byte? HadA1CTest { get; set; }

        public float? A1CValue { get; set; }

        public byte? DiabetesType { get; set; }

        [Required]
        public byte? HasDiabetes { get; set; }

        public byte? HasPreDiabetes { get; set; }

        public byte? EligibilityStatus { get; set; }

        [Required]
        public float? Height { get; set; }

        [Required]
        public float? Weight { get; set; }

        public byte? NoA1cTestReason { get; set; }

        public int? CreatedBy { get; set; }

        public List<string> Validate()
        {
            List<string> missingDataFields = new List<string>();
            if (HasDiabetes.HasValue)
            {
                if (HasDiabetes.Value == 1)
                {
                    if (!DiabetesType.HasValue)
                        missingDataFields.Add("DiabetesType");
                    if (string.IsNullOrEmpty(DiabetesDate))
                        missingDataFields.Add("DiabetesDate");
                }
                else if (HasDiabetes.Value == 2)
                {
                    if (!HasPreDiabetes.HasValue)
                        missingDataFields.Add("HasPreDiabetes");
                }
            }
            if (TakeDiabetesMed.HasValue && (TakeDiabetesMed.Value == 1))
            {
                if (!TakeInsulin.HasValue)
                    missingDataFields.Add("TakeInsulin");
            }
            if (HadA1CTest.HasValue && (HadA1CTest.Value == 1))
            {
                if (!A1CValue.HasValue)
                    missingDataFields.Add("A1CValue");
                if (string.IsNullOrEmpty(A1CTestDate))
                    missingDataFields.Add("A1CTestDate");
            }
            else
            {
                if (!NoA1cTestReason.HasValue)
                    missingDataFields.Add("NoA1cTestReason");
            }
            return missingDataFields;
        }

        public string DiabetesDate { get; set; }

        public string A1CTestDate { get; set; }

        public virtual EligibilityDto Eligibility { get; set; }

        public virtual HRADto HRA { get; set; }

        public virtual OrganizationDto Organization { get; set; }

        public DateTime? OptingOut { get; set; }

        public DateTime? PairedDate { get; set; }

        public DateTime? PatternsRegDate { get; set; }

        public string Devices { get; set; }

        public static DAL.IntuityEligibilityLog MapToIntuityEligibilityLog(DAL.IntuityEligibilityLog source)
        {
            var dal = new DAL.IntuityEligibilityLog();
            dal.FirstName = source.FirstName;
            dal.LastName = source.LastName;
            dal.DOB = source.DOB;
            dal.email = source.email;
            dal.PhoneNumber = source.PhoneNumber;
            dal.AddressLine1 = source.AddressLine1;
            dal.AddressLine2 = source.AddressLine2;
            dal.City = source.City;
            dal.State = source.State;
            dal.Country = source.Country;
            dal.Zip = source.Zip;
            dal.Gender = source.Gender;
            dal.UniqueId = source.UniqueId;
            dal.OrganizationCode = source.OrganizationCode;
            dal.OrganizationId = source.OrganizationId;
            dal.EligibilityId = source.EligibilityId;
            dal.APIStatus = source.APIStatus;
            dal.HRAId = source.HRAId;
            dal.EligibilityReason = source.EligibilityReason;
            dal.TakeDiabetesMed = source.TakeDiabetesMed;
            dal.TakeInsulin = source.TakeInsulin;
            dal.HadA1CTest = source.HadA1CTest;
            dal.A1CValue = source.A1CValue;
            dal.DiabetesType = source.DiabetesType;
            dal.HasDiabetes = source.HasDiabetes;
            dal.HasPreDiabetes = source.HasPreDiabetes;
            dal.EligibilityStatus = source.EligibilityStatus;
            dal.Height = source.Height;
            dal.Weight = source.Weight;
            dal.NoA1cTestReason = source.NoA1cTestReason;
            dal.DiabetesDate = source.DiabetesDate;
            dal.A1CTestDate = source.A1CTestDate;
            dal.Eligibility = source.Eligibility;
            dal.HRA = source.HRA;
            dal.Organization = source.Organization;
            dal.OptingOut = source.OptingOut;
            dal.PairedDate = source.PairedDate;
            dal.PatternsRegDate = source.PatternsRegDate;
            dal.Devices = source.Devices;

            return dal;
        }
    }
}
