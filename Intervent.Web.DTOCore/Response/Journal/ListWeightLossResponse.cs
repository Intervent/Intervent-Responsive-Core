namespace Intervent.Web.DTO
{
    public class ListWeightLossResponse
    {
        public IList<WeightLossJournalDto> weightLossList { get; set; }

        public IList<WeightLossJournalDto> weightLossLists { get; set; }

        public int totalRecords { get; set; }
    }
}
