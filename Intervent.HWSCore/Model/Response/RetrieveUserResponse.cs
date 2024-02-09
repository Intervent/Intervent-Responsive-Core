namespace Intervent.HWS
{

    public class GetUserByIdResponse
    {
        public GetUser[] users { get; set; }
    }

    public class GetUserTokenReponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public GetUser user { get; set; }
    }

    public class GetUser
    {
        public string _id { get; set; }
        public string uid { get; set; }
        public string authentication_token { get; set; }
    }

}
