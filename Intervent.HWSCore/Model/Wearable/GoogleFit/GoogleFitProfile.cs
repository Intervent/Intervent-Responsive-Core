namespace Intervent.HWS.Model
{
    public class GoogleFitProfile : ProcessResponse
    {
        public string family_name { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
        public string locale { get; set; }
        public string given_name { get; set; }
        public string id { get; set; }
    }
}
