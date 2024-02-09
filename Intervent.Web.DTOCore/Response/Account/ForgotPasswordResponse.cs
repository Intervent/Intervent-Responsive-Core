namespace Intervent.Web.DTO
{
    public class ForgotPasswordResponse
    {
        public string resetToken { get; set; }

        public string Error { get; set; }
        public UserDto user { get; set; }

    }
}