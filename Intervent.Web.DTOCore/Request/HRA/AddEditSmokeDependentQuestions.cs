namespace Intervent.Web.DTO
{
    public class AddEditSmokeDependentQuestions : IUserHistoryChange
    {
        public int HRAId { get; set; }

        public int UserId { get; set; }

        public int UpdatedByUserId { get; set; }

        public bool StoreHistory { get; set; }

        public bool isSmoker { get; set; }
    }
}