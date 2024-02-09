using System.Security.Claims;

namespace Intervent.Web.DTO
{
    public class CreateUserIdentityResponse
    {
        public ClaimsIdentity claimsIdentity { get; set; }
    }
}