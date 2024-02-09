namespace Intervent.Web.DTO
{
    public class AddEditOtherRiskFactorsRequest : IUserHistoryChange
    {
        public OtherRiskFactorsDto OtherRiskFactors { get; set; }

        public int UserId { get; set; }

        public int UpdatedByUserId { get; set; }

        public bool StoreHistory { get; set; }

        public bool IsIntuityUser { get; set; }
    }
}