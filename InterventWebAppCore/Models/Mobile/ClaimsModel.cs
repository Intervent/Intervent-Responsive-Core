namespace InterventWebApp
{
    public class ClaimsModel
    {
        public int userId { get; set; }
        public string deviceId { get; set; }
        public DateTime expiresIn { get; set; }
    }
}