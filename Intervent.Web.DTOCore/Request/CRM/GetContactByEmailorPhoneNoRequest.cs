namespace Intervent.Web.DTO
{
    public class GetContactByEmailorPhoneNoRequest
    {
        public string phoneNo { get; set; }

        public string email { get; set; }

        public bool byEmailorPhone { get; set; }
    }
}
