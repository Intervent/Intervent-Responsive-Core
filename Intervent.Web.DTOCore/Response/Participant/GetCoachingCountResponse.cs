namespace Intervent.Web.DTO
{
    public class GetCoachingCountResponse
    {
        public IList<NotesDto> participantNotes { get; set; }

        public int count { get; set; }
    }
}
