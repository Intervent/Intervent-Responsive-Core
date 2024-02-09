namespace Intervent.Web.DTO
{
    public class AddEditHealthNumbersRequest : IUserHistoryChange
    {
        public HealthNumbersDto HealthNumbers { get; set; }

        public bool? addtoWellnessData { get; set; }

        public int UserId { get; set; }

        public int UpdatedByUserId { get; set; }

        public bool StoreHistory { get; set; }

        public bool bloodwork { get; set; }

        public bool IsIntuityUser { get; set; }
    }
}