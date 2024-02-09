namespace Intervent.Web.DTO
{
    public class UserDoctorInfoDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public byte? ContactPermission { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        public string Zip { get; set; }

        public string FaxNumber { get; set; }

        public string PhoneNumber { get; set; }

        public bool? Active { get; set; }

        public int? ProviderId { get; set; }

        public StateDto State1 { get; set; }

        public CountryDto Country1 { get; set; }

        public virtual ProviderDto Provider { get; set; }
    }
}
