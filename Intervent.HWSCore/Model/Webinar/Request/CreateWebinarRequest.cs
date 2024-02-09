namespace Intervent.HWS
{
    public class CreateWebinarRequest
    {
        public string agenda { get; set; }

        public int duration { get; set; }

        public string password { get; set; }

        public DateTime start_time { get; set; }

        public string timezone { get; set; }

        public string topic { get; set; }

        public int type { get; set; }

        public Recurrence recurrence { get; set; }
    }

    public class Recurrence
    {
        public DateTime end_date_time { get; set; }

        public int end_times { get; set; }

        public int monthly_day { get; set; }

        public int monthly_week { get; set; }

        public int monthly_week_day { get; set; }

        public int repeat_interval { get; set; }

        public int type { get; set; }

        public string weekly_days { get; set; }
    }

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

    public class Interpreter
    {
        public string email { get; set; }

        public string languages { get; set; }
    }

    public class LanguageInterpretation
    {
        public bool enable { get; set; }

        public List<Interpreter> interpreters { get; set; }
    }

    public class QuestionAndAnswer
    {
        public bool allow_anonymous_questions { get; set; }

        public string answer_questions { get; set; }

        public bool attendees_can_comment { get; set; }

        public bool attendees_can_upvote { get; set; }

        public bool enable { get; set; }
    }

    public class Settings
    {
        public bool allow_multiple_devices { get; set; }

        public string alternative_hosts { get; set; }

        public bool alternative_host_update_polls { get; set; }

        public int approval_type { get; set; }

        public AttendeesAndPanelistsReminderEmailNotification attendees_and_panelists_reminder_email_notification { get; set; }

        public string audio { get; set; }

        public string authentication_domains { get; set; }

        public string authentication_option { get; set; }

        public string auto_recording { get; set; }

        public bool close_registration { get; set; }

        public string contact_email { get; set; }

        public string contact_name { get; set; }

        public string email_language { get; set; }

        public bool enforce_login { get; set; }

        public string enforce_login_domains { get; set; }

        public FollowUpAbsenteesEmailNotification follow_up_absentees_email_notification { get; set; }

        public FollowUpAttendeesEmailNotification follow_up_attendees_email_notification { get; set; }

        public List<string> global_dial_in_countries { get; set; }

        public bool hd_video { get; set; }

        public bool hd_video_for_attendees { get; set; }

        public bool host_video { get; set; }

        public LanguageInterpretation language_interpretation { get; set; }

        public bool meeting_authentication { get; set; }

        public bool on_demand { get; set; }

        public bool panelists_invitation_email_notification { get; set; }

        public bool panelists_video { get; set; }

        public bool post_webinar_survey { get; set; }

        public bool practice_session { get; set; }

        public QuestionAndAnswer question_and_answer { get; set; }

        public bool registrants_email_notification { get; set; }

        public int registrants_restrict_number { get; set; }

        public int registration_type { get; set; }

        public bool send_1080p_video_to_attendees { get; set; }

        public bool show_share_button { get; set; }

        public string survey_url { get; set; }

        public bool enable_session_branding { get; set; }
    }

    public class TrackingField
    {
        public string field { get; set; }

        public string value { get; set; }
    }
}
