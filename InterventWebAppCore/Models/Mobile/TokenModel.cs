namespace InterventWebApp
{
    public class TokenModel
    {
        public string? username { get; set; }
        public string? password { get; set; }
        public string? token { get; set; }
        public string? deviceId { get; set; }
        public string grant_type { get; set; }
        public string? refresh_token { get; set; }
    }
}