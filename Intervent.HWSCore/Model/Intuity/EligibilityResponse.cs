namespace Intervent.HWS
{
    public class EligibilityResponse : ProcessResponse
    {
        public int user_id { get; set; }
        public bool is_eligible { get; set; }
        public bool is_coaching_active { get; set; }
        //TBD: can INTERVENT provide this info?
        public bool is_manually_eligible { get; set; }
        public string auth_token { get; set; }
        //TBD: agree date format (ISO8601)
        public string expires_on { get; set; }
        public byte eligibility_type { get; set; }
        public string UniqueId { get; set; }
    }
}
