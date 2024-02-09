using Intervent.DAL;

namespace Intervent.Web.DTO
{
    public class GetUserResponse
    {
        public UserDto User { get; set; }

        public ApplicationUser appUser { get; set; }

        public bool? EmailConfirmed { get; set; }

        public string error { get; set; }

        public bool IsVerifiedDevice { get; set; }

        public string EmailId { get; set; }

        public string PhoneNo { get; set; }

        public int UserId { get; set; }
    }

    public class VerifyUserResponse
    {
        public string error { get; set; }

        public bool status { get; set; }
    }
}