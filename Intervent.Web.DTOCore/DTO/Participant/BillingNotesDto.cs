namespace Intervent.Web.DTO
{
    public class BillingNotesDto
    {
        public int Id { get; set; }

        public int WellnessId { get; set; }

        public int TimeSpent { get; set; }

        public bool Submitted { get; set; }

        public DateTime? SubmittedOn { get; set; }

        public string JsonRequest { get; set; }

        public virtual WellnessDataDto WellnessData { get; set; }
    }
}