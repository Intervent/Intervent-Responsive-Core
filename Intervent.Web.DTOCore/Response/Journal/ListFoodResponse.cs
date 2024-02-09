namespace Intervent.Web.DTO
{
    public class ListFoodDairyResponse
    {
        public IList<FoodDiaryDto> FoodDiaryList { get; set; }

        public IList<string> fg1RecentItems { get; set; }

        public IList<string> fg2RecentItems { get; set; }

        public IList<string> fg3RecentItems { get; set; }

        public IList<string> fg4RecentItems { get; set; }

        public IList<string> fg5RecentItems { get; set; }

        public IList<string> fg6RecentItems { get; set; }
    }
}