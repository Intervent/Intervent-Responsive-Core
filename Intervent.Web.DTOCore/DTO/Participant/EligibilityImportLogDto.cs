namespace Intervent.Web.DTO
{
    public sealed class EligibilityImportLogDto
    {
        public int Id { get; set; }

        public string UniqueId { get; set; }

        public int PortalId { get; set; }

        public string Action { get; set; }

        public DateTime LogDate { get; set; }

        public string CreatedByUser { get; set; }

        public string ChangedFields { get; set; }

        public int? EligibilityId { get; set; }

        public bool IsLoadError { get; set; }

        public string ErrorDetails { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}
