
namespace Intervent.HWS
{
    public class ProfileUpdate
    {
        public int user_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string gender { get; set; } // m, f, o
        public string date_of_birth { get; set; }
        public int diabetes_type { get; set; } // 1 or 2
        public string time_zone { get; set; }
        public string locale { get; set; }
        public string email_address { get; set; }
        public phoneNumbers[] phone_numbers { get; set; }
        public string address_one { get; set; }
        public string address_two { get; set; }
        public string city_village { get; set; }
        public string state_province { get; set; }
        public string Postal_code { get; set; }
        public string country { get; set; }
        //public insulins[] insulins { get; set; }
        public string[] medication_types { get; set; } // oral, insulin, injectable, pump, inhaled
        public healthCareProviders[] health_care_providers { get; set; }
        public testingWindows[] testing_windows { get; set; }
        //TBD: need a list of possible options, e.g. employer, selfinsured
        public string insurance_provider_type { get; set; }
        public string employer_name { get; set; }
        public string employee_id { get; set; }
        public string insurance_company_name { get; set; }
        public string insurance_policy_number { get; set; }
        public devices[] devices { get; set; }
        //TBD: need a list of possible options, e.g. self, spouse, child, dependent
        public string relationship { get; set; }
    }

    public class phoneNumbers
    {
        public string label { get; set; }
        public string number { get; set; }
        public bool preferred { get; set; }
    }
    public class insulins
    {
        //pump: String { format: /^.{1,50}$/, max: 12},
        public string pump { get; set; }
        //   inhaled: { format: /^.{1,50}$/, max: 12}
        public string inhaled { get; set; }

        //injectableFast: { format: /^.{1,50}$/, max: 12}
        public string injectable_fast { get; set; }

        //  injectableLong: { format: /^.{1,50}$/, max: 12}
        public string injectable_long { get; set; }
    }
    public class healthCareProviders
    {
        public int id { get; set; }
        public string salutation { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string specialty { get; set; }
        public string email_address { get; set; }
        public phoneNumbers[] phone_numbers { get; set; }
    }
    public class testingWindows
    {
        public int id { get; set; }
        public string icon { get; set; }
        public string label { get; set; }
        public string end_time { get; set; }
        public string start_time { get; set; }
        public bool enabled { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
    public class devices
    {
        public string serialNumber { get; set; }
    }
}
