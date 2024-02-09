using Intervent.DAL;

namespace Intervent.Web.DTO
{
    public class BulkUpdateIntuityQuantityOnHandListRequest
    {
        public IList<IntuityQOH> IntuityQOHRequestsList { get; set; }
    }
}
