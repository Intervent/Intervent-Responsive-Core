namespace Intervent.Web.DTO
{
    public class EligibilityNotesDto
    {
        public int Id { get; set; }

        public string UniqueId { get; set; }

        public int OrganizationId { get; set; }

        public int ModuleType { get; set; }

        public int NoteType { get; set; }

        public string Notes { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool Active { get; set; }

        public UserDto User { get; set; }
    }
}
