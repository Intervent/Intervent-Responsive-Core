namespace Intervent.HWS.Model
{
    public class WithingsNonce : ProcessResponse
    {
        public int status { get; set; }
        public Body body { get; set; }
        public string error { get; set; }

        public class Body
        {
            public string nonce { get; set; }
        }
    }
}
