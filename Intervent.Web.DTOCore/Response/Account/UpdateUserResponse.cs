using Microsoft.AspNetCore.Identity;

namespace Intervent.Web.DTO
{
    public class UpdateUserResponse
    {
        public bool Succeeded { get; set; }

        public bool primaryFieldsChanged { get; set; }

        public bool activeStatusChanged { get; set; }

        public IEnumerable<IdentityError> error { get; set; }

        public string updateUniqueIdResponse { get; set; }

		public bool isClearProgramRelatedSessions { get; set; }
	}
}