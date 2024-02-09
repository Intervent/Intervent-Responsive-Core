using System.ComponentModel.DataAnnotations;

namespace Intervent.Web.DTO
{
    public sealed class IntuityEligibilityCsvModel
    {
        [MaxLength(100)]
        public string UniqueId { get; set; }

        [MaxLength(128)]
        public string FirstName { get; set; }

        [MaxLength(128)]
        public string LastName { get; set; }

        [MaxLength(64)]
        public string OrganizationCode { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string MiddleName { get; set; }

        public bool? HasDiabetes { get; set; }

        public DiabeticType DiabetesType { get; set; }

        public int? Diabeteyear { get; set; }

        public bool? TakeDiabeteMed { get; set; }

        public bool? TakeInsulin { get; set; }

        public bool? HadA1CTest { get; set; }

        public float? A1CValue { get; set; }

        public BoolState TookA1C { get; set; }

        public DateTime DOB { get; set; }

        public GenderDto Gender { get; set; }

        public bool Valid()
        {
            if (string.IsNullOrEmpty(this.OrganizationCode) || string.IsNullOrEmpty(this.FirstName) || string.IsNullOrEmpty(this.LastName) || string.IsNullOrEmpty(this.Email) || DOB == DateTime.MinValue)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// If the user has answered yes to take diabete medication then send elgibility record
        /// </summary>
        /// <returns>true/false</returns>
        public bool HasDiabete()
        {
            if (this.TakeDiabeteMed.HasValue && this.TakeDiabeteMed.Value)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// If the user's A1C value is out of range then send the eligility record
        /// </summary>
        /// <returns>true/false</returns>
        public bool SatisfiesA1CRange()
        {
            // do we need to check year
            if (this.TookA1C == BoolState.Yes && this.A1CValue.HasValue && this.A1CValue.Value >= 6.5)
            {
                return true;
            }
            return false;
        }

        public IntuityEligibilityLogDto1 MapToIntuityEligibilityLog()
        {
            IntuityEligibilityLogDto1 dto = new IntuityEligibilityLogDto1();
            dto.UniqueId = this.UniqueId;
            dto.OrganizationCode = this.OrganizationCode;
            dto.Email = this.Email;
            dto.FirstName = this.FirstName;
            dto.LastName = this.LastName;
            dto.DOB = this.DOB;
            dto.OrganizationCode = this.OrganizationCode;
            dto.HasDiabetes = this.HasDiabetes;
            dto.Diabeteyear = this.Diabeteyear;
            dto.TakeDiabeteMed = this.TakeDiabeteMed;
            dto.TakeInsulin = this.TakeInsulin;
            dto.HadA1CTest = this.HadA1CTest;
            dto.A1CValue = this.A1CValue;
            dto.Gender = this.Gender;
            dto.DiabetesType = this.DiabetesType;
            dto.TookA1C = this.TookA1C;
            return dto;
        }
    }

}
