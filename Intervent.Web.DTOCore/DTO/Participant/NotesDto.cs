namespace Intervent.Web.DTO
{
    public class NotesDto
    {
        public int Id { get; set; }

        public int userId { get; set; }

        public byte Type { get; set; }

        public string Text { get; set; }

        public DateTime NotesDate { get; set; }

        public int Admin { get; set; }

        public int PortalId { get; set; }

        public int? RefId { get; set; }

        public PortalDto Portal { get; set; }

        public UserDto User { get; set; }

        public UserDto User1 { get; set; }

        public bool Pinned { get; set; }

        public int? RefId2 { get; set; }
    }
}