namespace Intervent.HWS
{
    public class VerifyIntuityUserRequest : ProcessResponse
    {
        public string emailId { get; set; }

        public string shopifyCustomerNumber { get; set; }

        public string UniqueId { get; set; }
    }
}
