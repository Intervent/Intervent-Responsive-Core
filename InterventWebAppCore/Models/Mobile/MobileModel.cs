namespace InterventWebApp
{
    public class UserIdentity
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Module { get; set; }

        public string RoleCode { get; set; }

        public string TimeZone { get; set; }

        public string TimeZoneName { get; set; }

        public string ExpirationUrl { get; set; }

        public string DeviceId { get; set; }

        public string Token { get; set; }

        public byte Gender { get; set; }

        public int Unit { get; set; }

        public string DateFormat { get; set; }

        public bool SingleSignOn { get; set; }

        public bool MobileSignOn { get; set; }
    }

    public class NotificationsResponse
    {
        public IList<Notification> notification { get; set; }
    }

    public class Notification
    {
        public int message_id { get; set; }

        public string message { get; set; }

        public string icon_url { get; set; }

        public string date { get; set; }

        public string destination_url { get; set; }
    }

    public class FeedsResponse
    {
        public IList<Feed> feed { get; set; }
    }

    public class Feed
    {
        public int message_id { get; set; }

        public string message { get; set; }

        public string icon_url { get; set; }

        public string date { get; set; }

        public string destination_url { get; set; }
    }

    public class DeviceResponse
    {
        public string url { get; set; }

        public string external_id { get; set; }

        public string organization_id { get; set; }

        public string access_token { get; set; }

        public string mobile_token { get; set; }

        public int connected_device_count { get; set; }
    }

    public class WearableDevicesResponse
    {
        public string type { get; set; }

        public bool connected { get; set; }

        public string logo_url { get; set; }

        public string display_name { get; set; }

        public string connect_url { get; set; }

        public string disconnect_url { get; set; }
    }

    public class WearableResponse
    {
        public List<WearableDevicesResponse> wearables { get; set; }

        public int connected_device_count { get; set; }

        public bool is_google_fit_connected { get; set; }

        public string google_fit_external_id { get; set; }

        public bool is_apple_health_connected { get; set; }

        public string apple_health_external_id { get; set; }
    }

    public class UserProfile
    {
        public string first_name { get; set; }

        public string middle_name { get; set; }

        public string last_name { get; set; }

        public string phone { get; set; }

        public string dob { get; set; }

        public string date_format { get; set; }

        public string email { get; set; }

        public string gender { get; set; }

        public string address1 { get; set; }

        public string address2 { get; set; }

        public string city { get; set; }

        public int state_id { get; set; }

        public int country_id { get; set; }

        public string state { get; set; }

        public string country { get; set; }

        public string zip { get; set; }

        public string profile_picture { get; set; }

        public int time_zone { get; set; }

        public CoachProfile coach_profile { get; set; }

        public Settings settings { get; set; }
    }

    public class Settings
    {
        public string edit_profile_url { get; set; }

        public bool receive_text { get; set; }

        public bool mobile_notification { get; set; }

        public bool show_mobile_notification { get; set; }

        public IEnumerable<TimeZone> time_zones { get; set; }
    }

    public class StatesList
    {
        public IEnumerable<State> states;
    }

    public class CountriesList
    {
        public IEnumerable<Country> countries;
    }

    public class CoachProfile
    {
        public string first_name { get; set; }

        public string last_name { get; set; }

        public string gender { get; set; }

        public string role { get; set; }

        public string profile { get; set; }

        public string profile_picture { get; set; }
    }

    public class ChangePassword
    {
        public string old_password { get; set; }

        public string new_password { get; set; }
    }

    public class VerifyPassword
    {
        public string password { get; set; }
    }

    public class ForgotPassword
    {
        public string email { get; set; }
    }

    public class UpdateSettings
    {
        public bool receive_text { get; set; }

        public bool mobile_notification { get; set; }

        public string notification_token { get; set; }

        public int time_zone { get; set; }
    }

    public class ChangeEmail
    {
        public string email { get; set; }
    }

    public class ChangeEmailResponse
    {
        public bool status { get; set; }

        public string message { get; set; }
    }

    public class UserProfileRequest
    {
        public string first_name { get; set; }

        public string? middle_name { get; set; }

        public string last_name { get; set; }

        public string phone { get; set; }

        public DateTime? dob { get; set; }

        public string address1 { get; set; }

        public string? address2 { get; set; }

        public string city { get; set; }

        public int? state_id { get; set; }

        public int? country_id { get; set; }

        public string zip { get; set; }
    }

    public class MessageDashBoard
    {
        public int id { get; set; }

        public string subject { get; set; }

        public string message { get; set; }

        public string date { get; set; }

        public bool is_draft { get; set; }

        public bool is_sent { get; set; }

        public bool is_read { get; set; }

        public bool has_attachment { get; set; }

        public int unread_count { get; set; }

    }

    public class Message
    {
        public int id { get; set; }

        public int parent_message_id { get; set; }

        public string subject { get; set; }

        public string message { get; set; }

        public string date { get; set; }

        public bool is_user_message { get; set; }

        public bool is_draft { get; set; }

        public bool is_sent { get; set; }

        public bool is_read { get; set; }

        public bool can_delete { get; set; }

        public string status { get; set; }

        public string creator_name { get; set; }

        public string creator_role { get; set; }

        public string attachment_name { get; set; }

        public string attachment_url { get; set; }

        public string attachment_size { get; set; }
    }

    public class DashBoardMessageRequest
    {
        public bool drafts { get; set; }

        public bool unread { get; set; }

        public string? search_text { get; set; }
    }

    public class MessageRequest
    {
        public int message_id { get; set; }
    }

    public class AddMessageRequest
    {
        public int parent_message_id { get; set; }

        public int message_id { get; set; }

        public string subject { get; set; }

        public string message { get; set; }

        public string attachement { get; set; }

        public string attachement_name { get; set; }

        public bool is_sent { get; set; }
    }

    public class DeleteMessageRequest
    {
        public int message_id { get; set; }

        public string attachement_name { get; set; }
    }

    public class DashboardMessageRequest
    {
        public int message_id { get; set; }
    }

    public class StateRequest
    {
        public int country_id { get; set; }
    }

    public class DeviceRequest
    {
        public string device_type { get; set; }

        public string external_user_id { get; set; }

        public bool is_connected { get; set; }
    }
}
