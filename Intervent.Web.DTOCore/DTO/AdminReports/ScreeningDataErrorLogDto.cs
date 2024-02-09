namespace Intervent.Web.DTO
{
    public class ScreeningDataErrorLogDto
    {
        public int Id { get; set; }
        public int OrgId { get; set; }
        public string UniqueId { get; set; }
        public string Error { get; set; }
        public DateTime Date { get; set; }
        public OrganizationDto Organization { get; set; }
    }
}
