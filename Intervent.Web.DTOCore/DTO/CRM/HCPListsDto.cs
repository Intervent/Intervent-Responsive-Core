namespace Intervent.Web.DTO
{
    public class HCPListsDto
    {
        public int Id { get; set; }

        public int? OrganizationId { get; set; }

        public bool UserAdded { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public int State { get; set; }

        public string ZipCode { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public virtual OrganizationDto Organization { get; set; }

        public virtual StateDto State1 { get; set; }

    }
}
