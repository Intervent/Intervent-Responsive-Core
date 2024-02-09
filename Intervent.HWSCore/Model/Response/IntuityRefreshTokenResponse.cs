namespace Intervent.HWS
{
    public class IntuityRefreshTokenResponse : ProcessResponse
    {
        public RefreshToken refreshToken { get; set; }

        public class RefreshToken
        {
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public DateTime access_token_expiry_date { get; set; }
            public DateTime refresh_token_expiry_date { get; set; }

        }
    }
}
