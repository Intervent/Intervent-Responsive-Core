namespace Intervent.HWS
{
    public class GetWebinarResponse : ProcessResponse
    {
        public RegisterWebinarsResponse registerWebinars { get; set; }

        public class RegisterWebinarsResponse
        {
            public string page_count { get; set; }
            public string page_number { get; set; }
            public string page_size { get; set; }
            public string total_records { get; set; }
            public List<Registrant> registrants { get; set; }
        }
        public class CustomQuestion
        {
            public string title { get; set; }
            public string value { get; set; }
        }

        public class Registrant
        {
            public string id { get; set; }
            public string email { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public string zip { get; set; }
            public string state { get; set; }
            public string phone { get; set; }
            public string industry { get; set; }
            public string org { get; set; }
            public string job_title { get; set; }
            public string purchasing_time_frame { get; set; }
            public string role_in_purchase_process { get; set; }
            public string no_of_employees { get; set; }
            public string comments { get; set; }
            public List<CustomQuestion> custom_questions { get; set; }
            public string status { get; set; }
            public string create_time { get; set; }
            public string join_url { get; set; }
        }
    }
}
