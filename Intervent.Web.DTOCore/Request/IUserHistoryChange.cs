namespace Intervent.Web.DTO
{
    public interface IUserHistoryChange
    {
        int UserId { get; set; }
        int UpdatedByUserId { get; set; }
    }
}
