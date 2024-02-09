namespace InterventWebApp
{
    public class DashBoardModel
    {
        public BloodPressure blood_pressure { get; set; }

        public PhysicalActivity physical_activity { get; set; }

        public Glucose glucose { get; set; }

        public List<Link> links { get; set; }

        public DeviceAlert device_alert { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string profile_picture { get; set; }

        public string terms_and_conditions { get; set; }

        public string about_app_url { get; set; }

        public string webview_oauth_url { get; set; }

        public string expiration_url { get; set; }

        public string physical_activity_graph_url { get; set; }

        public string blood_pressure_graph_url { get; set; }

        public string glucose_graph_url { get; set; }

        public string physical_activity_header { get; set; }

        public string blood_pressure_header { get; set; }

        public string glucose_header { get; set; }

        public string graph_link_text { get; set; }

        public bool show_glucose_graph { get; set; }

        public string watch_video { get; set; }
    }

    public class Link
    {
        public string name { get; set; }

        public string url { get; set; }

        public string icon { get; set; }
    }

    public class DeviceAlert
    {
        public string type { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string icon_url { get; set; }
    }

    public class Glucose
    {
        public string unit { get; set; }

        public int average { get; set; }

        public string post_meal_color { get; set; }

        public string pre_meal_color { get; set; }

        public string random_color { get; set; }

        public string range_color { get; set; }

        public string range_text { get; set; }

        public int average_pre_meal { get; set; }

        public string pre_meal_range_color { get; set; }

        public string pre_meal_range_text { get; set; }

        public int average_post_meal { get; set; }

        public string post_meal_range_color { get; set; }

        public string post_meal_range_text { get; set; }

        public List<Graph> pre_meal { get; set; }

        public List<Graph> post_meal { get; set; }

        public List<Graph> random { get; set; }
    }

    public class Graph
    {
        public string time { get; set; }

        public double? value { get; set; }
    }

    public class PhysicalActivity
    {
        public int average_steps { get; set; }

        public int average_time { get; set; }

        public int average_calories { get; set; }

        public string steps_color { get; set; }

        public string steps_unit { get; set; }

        public string time_color { get; set; }

        public string time_unit { get; set; }

        public List<Activity> activity_time { get; set; }

        public List<Activity> activity_steps { get; set; }
    }

    public class Activity
    {
        public string date { get; set; }

        public int value { get; set; }
    }

    public class BloodPressure
    {
        public int average_diastolic { get; set; }

        public int average_systolic { get; set; }

        public string unit { get; set; }

        public string diastolic_color { get; set; }

        public string systolic_color { get; set; }

        public string range_color { get; set; }

        public string range_text { get; set; }

        public List<BP> blood_pressure { get; set; }
    }

    public class BP
    {
        public string date { get; set; }

        public double diastolic { get; set; }

        public double systolic { get; set; }
    }

    public class TimeZone
    {
        public int id { get; set; }

        public string name { get; set; }
    }

    public class Country
    {
        public int id { get; set; }

        public string name { get; set; }
    }

    public class State
    {
        public int id { get; set; }

        public string name { get; set; }

        public int country_id { get; set; }
    }
}
