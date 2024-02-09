namespace Intervent.Web.DTO
{
    public class UpdateIntuityFulfillmentResponse
    {
        public bool Status { get; set; }

        public IList<IntuityFulfillmentRequestsDto> IntuityFulfillmentRequests { get; set; }
    }
}
