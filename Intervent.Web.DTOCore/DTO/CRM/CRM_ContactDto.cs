namespace Intervent.Web.DTO
{
    public class CRM_ContactDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DOB { get; set; }

        public byte? Gender { get; set; }

        public int? OrganizationId { get; set; }

        public string PhoneNumber1 { get; set; }

        public string PhoneNumber2 { get; set; }

        public string PhoneNumber3 { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        public string Zip { get; set; }

        public string PogoMeterNumber { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public string Address2 { get; set; }

        public string City2 { get; set; }

        public int? State2 { get; set; }

        public int? Country2 { get; set; }

        public string Zip2 { get; set; }

        public string InsuranceType { get; set; }

        public virtual IList<CRM_ChangeLogDto> CRM_ChangeLogs { get; set; }

        public virtual IList<CRM_NoteDto> CRM_Notes { get; set; }

        public virtual UserDto User { get; set; }

        public virtual UserDto User1 { get; set; }

        public virtual OrganizationDto Organization { get; set; }

        public virtual CountryDto Countries { get; set; }

        public virtual StateDto States { get; set; }

        public virtual IList<CRM_PogoMeterNumbersDto> CRM_PogoMeterNumbers { get; set; }

        public virtual IList<InsuranceTypesDto> InsuranceTypes { get; set; }

        public string Notes { get; set; }

        public byte OptedIn { get; set; }

        public string UniqueId { get; set; }
    }
}
