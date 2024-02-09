namespace InterventWebApp
{
    public class BearerToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; } = "bearer";
        public int expires_in { get; set; } = 1199;
        public string refresh_token { get; set; }
    }
}