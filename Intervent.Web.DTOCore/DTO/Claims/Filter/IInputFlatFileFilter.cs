namespace Intervent.Web.DTO.DTO.Claims.Filter
{
    public interface IInputFlatFileFilter
    {
        bool FilterRecord(ClaimsInputFlatFileModel record);

        string FilterMessage { get; }
    }
}
