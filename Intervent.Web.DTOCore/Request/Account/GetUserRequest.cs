namespace Intervent.Web.DTO
{
    public class GetUserRequest
    {
        public string password { get; set; }

        public int? id { get; set; }

        public string userName { get; set; }

        public bool? validateUser { get; set; }

        public string deviceID { get; set; }

        public bool appLogin { get; set; }

        public bool verifyDeviceLogin { get; set; }
    }

    public class VerifyUserRequest
    {
        public string password { get; set; }

        public string userName { get; set; }

        public string deviceID { get; set; }
    }

    public class GetUserRequestByUniqueId
    {
        public int? OrganizationId { get; set; }

        public string UniqueId { get; set; }
    }
}
