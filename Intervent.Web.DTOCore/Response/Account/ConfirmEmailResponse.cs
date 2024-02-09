using Microsoft.AspNetCore.Identity;

namespace Intervent.Web.DTO
{
    public class ConfirmEmailResponse
    {
        public bool Succeeded { get; set; }

        public IEnumerable<IdentityError> error { get; set; }
    }
}