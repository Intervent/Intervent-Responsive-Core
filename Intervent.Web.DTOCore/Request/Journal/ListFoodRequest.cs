namespace Intervent.Web.DTO
{
    public class ListFoodDiaryRequest
    {
        public int ParticipantId { get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }
    }
}