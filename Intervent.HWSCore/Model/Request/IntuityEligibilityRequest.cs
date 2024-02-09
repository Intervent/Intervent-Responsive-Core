
namespace Intervent.HWS
{
    public class IntuityEligibilityRequest
    {
        public string unique_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string has_diabetes { get; set; }
        public string diabetes_type { get; set; }
        public string diabetes_diagnosis_date { get; set; }
        public string has_prediabetes { get; set; }
        public string take_diabetes_medication { get; set; }
        public string take_insulin { get; set; }
        public string had_a1c_test { get; set; }
        public double a1c_value { get; set; }
        public string a1c_test_date { get; set; }
        public double height { get; set; }
        public double weight { get; set; }
        public string no_a1c_test_reason { get; set; }
    }
}
