using Microsoft.AspNetCore.Identity;

namespace Intervent.Web.DTO
{
    public class RegisterUserResponse
    {
        public bool Succeeded { get; set; }

        public int userId { get; set; }

        public string Token { get; set; }

        public string EmailId { get; set; }

        public IEnumerable<IdentityError> error { get; set; }
    }
}