namespace Intervent.Web.DTO
{
    public class GetNotesResponse
    {
        public IList<NotesDto> participantNotes { get; set; }

        public NotesDto participantNote { get; set; }
    }
}