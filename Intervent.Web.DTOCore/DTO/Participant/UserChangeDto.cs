namespace Intervent.Web.DTO
{
    public class UserHistoryDto
    {
        public long Id { get; set; }

        public int UserId { get; set; }

        public int UpdatedBy { get; set; }

        public string Changes { get; set; }

        public int UserHistoryCategoryId { get; set; }

        public DateTime LogDate { get; set; }

        public string UpdatedByName { get; set; }

    }
}
