namespace Intervent.Web.DTO
{
    public partial class ProviderDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        public string Zip { get; set; }

        public string FaxNumber { get; set; }

        public string PhoneNumber { get; set; }

        public bool Active { get; set; }

        public int? OrgId { get; set; }

        public CountryDto Country1 { get; set; }

        public virtual OrganizationDto Organization { get; set; }

        public StateDto State1 { get; set; }
    }
}
