namespace Intervent.HWS
{
    public class WebinarResponse : ProcessResponse
    {
        public Webinar webinar { get; set; }

        public class AttendeesAndPanelistsReminderEmailNotification
        {
            public bool enable { get; set; }
            public int type { get; set; }
        }

        public class FollowUpAbsenteesEmailNotification
        {
            public bool enable { get; set; }
            public int type { get; set; }
        }

        public class FollowUpAttendeesEmailNotification
        {
            public bool enable { get; set; }
            public int type { get; set; }
        }

        public class GlobalDialInNumber
        {
            public string country_name { get; set; }
            public string number { get; set; }
            public string type { get; set; }
            public string country { get; set; }
            public string city { get; set; }
        }

        public class QuestionAndAnswer
        {
            public bool enable { get; set; }
            public bool allow_anonymous_questions { get; set; }
            public string answer_questions { get; set; }
            public bool attendees_can_upvote { get; set; }
            public bool attendees_can_comment { get; set; }
        }

        public class Recurrence
        {
            public int type { get; set; }
            public int repeat_interval { get; set; }
            public int end_times { get; set; }
        }

        public class Occurrence
        {
            public string occurrence_id { get; set; }
            public DateTime start_time { get; set; }
            public int duration { get; set; }
            public string status { get; set; }
        }

        public class Webinar
        {
            public string uuid { get; set; }
            public long id { get; set; }
            public string host_id { get; set; }
            public string host_email { get; set; }
            public string topic { get; set; }
            public int type { get; set; }
            public DateTime start_time { get; set; }
            public int duration { get; set; }
            public string timezone { get; set; }
            public string agenda { get; set; }
            public DateTime created_at { get; set; }
            public string start_url { get; set; }
            public string join_url { get; set; }
            public string registration_url { get; set; }
            public string password { get; set; }
            public List<Occurrence> occurrences { get; set; }
            public Settings settings { get; set; }
            public Recurrence recurrence { get; set; }
            public bool is_simulive { get; set; }
        }

        public class Settings
        {
            public bool host_video { get; set; }
            public bool panelists_video { get; set; }
            public int approval_type { get; set; }
            public int registration_type { get; set; }
            public string audio { get; set; }
            public string auto_recording { get; set; }
            public bool enforce_login { get; set; }
            public string enforce_login_domains { get; set; }
            public string alternative_hosts { get; set; }
            public bool alternative_host_update_polls { get; set; }
            public bool close_registration { get; set; }
            public bool show_share_button { get; set; }
            public bool allow_multiple_devices { get; set; }
            public bool practice_session { get; set; }
            public bool hd_video { get; set; }
            public bool question_answer { get; set; }
            public bool registrants_confirmation_email { get; set; }
            public bool on_demand { get; set; }
            public bool request_permission_to_unmute_participants { get; set; }
            public List<string> global_dial_in_countries { get; set; }
            public List<GlobalDialInNumber> global_dial_in_numbers { get; set; }
            public string contact_name { get; set; }
            public string contact_email { get; set; }
            public int registrants_restrict_number { get; set; }
            public bool registrants_email_notification { get; set; }
            public bool post_webinar_survey { get; set; }
            public bool meeting_authentication { get; set; }
            public QuestionAndAnswer question_and_answer { get; set; }
            public bool hd_video_for_attendees { get; set; }
            public bool send_1080p_video_to_attendees { get; set; }
            public bool panelist_authentication { get; set; }
            public string email_language { get; set; }
            public bool panelists_invitation_email_notification { get; set; }
            public AttendeesAndPanelistsReminderEmailNotification attendees_and_panelists_reminder_email_notification { get; set; }
            public FollowUpAttendeesEmailNotification follow_up_attendees_email_notification { get; set; }
            public FollowUpAbsenteesEmailNotification follow_up_absentees_email_notification { get; set; }
            public bool enable_session_branding { get; set; }
        }
    }
}
