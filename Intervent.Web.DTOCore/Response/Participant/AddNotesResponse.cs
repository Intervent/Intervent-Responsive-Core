namespace Intervent.Web.DTO
{
    public class AddNotesResponse
    {
        public bool success { get; set; }

        public bool HasNote { get; set; }

        public bool NoProgram { get; set; }

        public bool? notesPage { get; set; }
    }
}