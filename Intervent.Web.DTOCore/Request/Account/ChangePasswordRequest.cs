namespace Intervent.Web.DTO
{
    public class ChangePasswordRequest
    {
        public int UserId { get; set; }

        public string oldPassword { get; set; }

        public string newPassword { get; set; }
    }
}
