namespace InterventWebApp
{
    public class DailyVitals
    {
        public DailyVitals()
        {
            daily_vitals = new List<VitalsLog>();
        }

        public int vitals_id { get; set; }

        public bool has_pending_vitals { get; set; }

        public List<VitalsLog> daily_vitals { get; set; }

        public string assessment_header { get; set; }

        public string assessment_title { get; set; }

        public string assessment_description { get; set; }

        public string assessment_button { get; set; }

    }

    public class VitalsLog
    {
        public VitalsLog()
        {
            answer_types = new List<AnswerType>();
        }

        public int question_no { get; set; }

        public string question_text { get; set; }

        public string answer { get; set; }

        public List<AnswerType> answer_types { get; set; }

        public Validation validiation { get; set; }
    }

    public class AnswerType
    {
        public string text { get; set; }

        public string type { get; set; }

        public string value { get; set; }
    }

    public class Validation
    {
        public float min { get; set; }

        public float max { get; set; }
    }

    public class SaveDailyVitals
    {
        public int vitals_id { get; set; }

        public string question_no { get; set; }

        public string answer { get; set; }
    }

    public class SaveDailyVitalsResponse
    {
        public bool success { get; set; }

        public int vitals_id { get; set; }

        public bool has_pending_vitals { get; set; }
    }
}
