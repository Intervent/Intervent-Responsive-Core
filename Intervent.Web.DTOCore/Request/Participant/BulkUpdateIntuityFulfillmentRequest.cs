using Intervent.DAL;

namespace Intervent.Web.DTO
{
    public class BulkUpdateIntuityFulfillmentRequest
    {
        public IList<IntuityFulfillmentRequests> IntuityFulfillmentRequestsList { get; set; }
    }
}
