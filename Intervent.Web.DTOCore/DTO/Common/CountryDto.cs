namespace Intervent.Web.DTO
{
    public class CountryDto
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string UNCode { get; set; }

        public string Name { get; set; }

        public string DateFormat { get; set; }

        public bool HasZipCode { get; set; }
    }
}