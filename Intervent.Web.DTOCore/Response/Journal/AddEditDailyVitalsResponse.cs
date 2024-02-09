namespace Intervent.Web.DTO
{
    public class AddEditDailyVitalsResponse
    {
        public bool success { get; set; }

        public int DailyVitalsId { get; set; }

        public bool hasPendingVitals { get; set; }

        public int vitalsCompletionPoints { get; set; }
    }
}
