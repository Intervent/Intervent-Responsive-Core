namespace Intervent.Web.DTO
{
    public class FollowUp_HealthConditionsDto
    {
        public int Id { get; set; }
        public byte? StateOfHealth { get; set; }
        public byte? ProductivityLoss { get; set; }

        public FollowUpDto FollowUp { get; set; }
    }
}
