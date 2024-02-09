namespace Intervent.HWS
{
    public class VerifyIntuityUserResponse : ProcessResponse
    {
        public int? user_id { get; set; }
        public string email_address { get; set; }
        public string shopify_customer_number { get; set; }
        public string unique_id { get; set; }
        public PhoneNUmbers[] phone_numbers { get; set; }
    }

    public class PhoneNUmbers
    {
        public string label { get; set; }
        public string number { get; set; }
        public bool preferred { get; set; }
    }
}
