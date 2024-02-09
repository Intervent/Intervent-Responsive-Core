namespace Intervent.HWS
{
    public class IntuityEligibilityLogAPIResponse : ProcessResponse
    {
        public IntuityAPIResponse intuityAPIResponse { get; set; }

        public class IntuityAPIResponse
        {
            public Patient patient { get; set; }
        }

        public class Pattern
        {
            public int id { get; set; }
            public object first_sync_date { get; set; }
            public object last_sync_date { get; set; }
        }

        public class Employer
        {
            public int id { get; set; }
            public string code { get; set; }
            public string business_unit_code { get; set; }
            public string business_unit_name { get; set; }
            public int tube_quantity { get; set; }
            public string contract_start_date { get; set; }
            public string contract_end_date { get; set; }
            public int second_shipment_wait_days { get; set; }
            public object welcome_email_sent_at { get; set; }
            public object last_promotional_email_sent_at { get; set; }
        }

        public class Patient
        {
            public int id { get; set; }
            public string patient_unique_id { get; set; }
            public int patient_lead_id { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string email { get; set; }
            public string address1 { get; set; }
            public object address2 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string zip { get; set; }
            public string eligibility_status { get; set; }
            public string patient_type { get; set; }
            public object status { get; set; }
            public object status_reason { get; set; }
            public object phone { get; set; }
            public object temporary_start_date { get; set; }
            public object temporary_end_date { get; set; }
            public object temporary_address1 { get; set; }
            public object temporary_address2 { get; set; }
            public object temporary_address3 { get; set; }
            public object temporary_city { get; set; }
            public object temporary_state { get; set; }
            public object temporary_zip { get; set; }
            public object temporary_country { get; set; }
            public object eligibility_flag { get; set; }
            public object last_shipment_tracking_number { get; set; }
            public object first_shipment_date { get; set; }
            public object last_shipment_date { get; set; }
            public object last_replenishment_calculation_date { get; set; }
            public object calculated_replenishment_quantity { get; set; }
            public object calc_replenishment_shipment_date { get; set; }
            public object coachcall_cartridge_qoh { get; set; }
            public object coachcall_date { get; set; }
            public object require_immediate_shipment { get; set; }
            public object custom_replenishment_quantity { get; set; }
            public object custom_replenishment_num_of_shipments { get; set; }
            public object custom_replenishment_qty_reason { get; set; }
            public object opting_out_date { get; set; }
            public object number_of_meters { get; set; }
            public object custom_replenishment_counter { get; set; }
            public object last_shipment_quantity { get; set; }
            public object coachcall_qtyoh_updated_at { get; set; }
            public object last_shipment_type { get; set; }
            public object last_shipment_service_type { get; set; }
            public object updated_at { get; set; }
            public object termination_date { get; set; }
            public object patient_lead_updated_at { get; set; }
            public int patterns_registration_reminders_count { get; set; }
            public int sync_test_result_reminders_count { get; set; }
            public int pair_monitor_reminders_count { get; set; }
            public Employer employer { get; set; }
            public List<Answer> answers { get; set; }
            public Pattern pattern { get; set; }
        }

        public class Answer
        {
            public int id { get; set; }
            public object question { get; set; }
            public object answer { get; set; }
            public object question_code { get; set; }
        }
    }
}
