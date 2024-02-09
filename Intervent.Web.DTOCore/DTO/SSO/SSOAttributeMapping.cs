namespace Intervent.Web.DTO
{
    public class SSOAttributeMappingDto
    {
        public int Id { get; set; }
        public int SSOProviderId { get; set; }
        public string AttributeName { get; set; }
        public int TypeId { get; set; }
        public string Format { get; set; }
        public int UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public enum AttributeType
    {
        GivenName = 1,
        SurName = 2,
        Postal = 3,
        Gender = 4,
        BirthDate = 5,
        Phone = 6,
        UniqueId = 7,
        Email = 8,
    }
}
