namespace Intervent.Web.DTO
{
    public class IntuityEligibilityLogDto1
    {
        public string UniqueId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        public string Zip { get; set; }

        public DateTime? DOB { get; set; }

        public GenderDto Gender { get; set; }

        public int? Id { get; set; }

        public int? OrganizationId { get; set; }


        public int? EligibilityId { get; set; }

        public string OrganizationCode { get; set; }

        public EligibilityReason EligibilityReason { get; set; }

        public int? HRAId { get; set; }

        public EligibilityStatus EligibilityStatus { get; set; }

        public bool? HasDiabetes { get; set; }

        public DiabeticType DiabetesType { get; set; }

        public int? Diabeteyear { get; set; }

        public bool? TakeDiabeteMed { get; set; }

        public bool? TakeInsulin { get; set; }

        public bool? HadA1CTest { get; set; }

        public float? A1CValue { get; set; }

        public DateTime? DiabetesDate { get; set; }

        public DateTime? A1CTestDate { get; set; }

        public BoolState TookA1C { get; set; }

        public DateTime? OptingOut { get; set; }

        public int? CreatedBy { get; set; }

        public DAL.IntuityEligibilityLog MapToIntuityEligibilityDAL()
        {
            DAL.IntuityEligibilityLog dal = new DAL.IntuityEligibilityLog();
            dal.UniqueId = this.UniqueId;
            dal.OrganizationId = this.OrganizationId;
            dal.EligibilityId = this.EligibilityId;
            dal.HRAId = this.HRAId;
            //   dal.Email = this.Email;
            dal.FirstName = this.FirstName;
            dal.LastName = this.LastName;
            if (this.DOB.HasValue)
            {
                dal.DOB = this.DOB.Value;
            }
            dal.OrganizationCode = this.OrganizationCode;
            //dal.HasDiabetes = this.HasDiabetes.HasValue;
            //dal.Diabeteyear = this.Diabeteyear;
            //dal.TakeDiabeteMed = this.TakeDiabeteMed;
            //dal.TakeInsulin = this.TakeInsulin;
            //dal.LastA1CTest = this.LastA1CTest;
            dal.A1CValue = this.A1CValue;

            if (this.EligibilityReason != EligibilityReason.None)
            {
                dal.EligibilityReason = (byte)(this.EligibilityReason);
            }

            dal.EligibilityStatus = (byte)(this.EligibilityStatus);

            if (this.Gender != null)
            {
                dal.Gender = this.Gender.Key;
            }
            if (this.DiabetesType != DiabeticType.None)
            {
                dal.DiabetesType = (byte)this.DiabetesType;
            }
            dal.DateCreated = DateTime.UtcNow;
            return dal;
        }
    }
}
