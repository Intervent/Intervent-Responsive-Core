namespace Intervent.HWS.Model
{
    public class WithingsRevokeUser : ProcessResponse
    {
        public int status { get; set; }
        public Body body { get; set; }
        public string error { get; set; }

        public class Body
        {
        }
    }
}
