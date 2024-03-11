namespace Intervent.HWS
{
    public class IntuityEligibilityLogAPIRequest
    {
        public string patient_unique_id { get; set; }

        public class IntuityEligibilityProfile
        {
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string email { get; set; }
            public string address1 { get; set; }
            public string address2 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string phone { get; set; }
            public string zip { get; set; }

        }

        public class IntuityEligibilityStatus
        {
            public string eligibility_status { get; set; }
            public string status_reason { get; set; }
        }

        public class Replenishment
        {
            public bool require_immediate_shipment { get; set; }
            public int custom_replenishment_quantity { get; set; }
            public string custom_replenishment_qty_reason { get; set; }
            public int custom_replenishment_counter { get; set; }
            public int custom_replenishment_num_of_shipments { get; set; }
        }

        public class QuantityOnHand
        {
            public int coachcall_cartridge_qoh { get; set; }
            public string coachcall_date { get; set; }
        }

        public class OptingOut
        {
            public string opting_out_date { get; set; }
        }
    }
}
