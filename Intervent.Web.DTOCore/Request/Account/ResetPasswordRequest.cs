namespace Intervent.Web.DTO
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }

        public string token { get; set; }

        public string Password { get; set; }

        public DateTime? DOB { get; set; }
    }
}