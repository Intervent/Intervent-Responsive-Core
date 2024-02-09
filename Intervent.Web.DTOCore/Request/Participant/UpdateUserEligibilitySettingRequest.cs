namespace Intervent.Web.DTO
{
    public class UpdateUserEligibilitySettingRequest
    {
        public UserEligibilitySettingDto UserEligibilitySetting { get; set; }

    }

    public class GetUserEligibilitySettingRequest
    {
        public string UniqueID { get; set; }

        public int OrgID { get; set; }

    }
}
