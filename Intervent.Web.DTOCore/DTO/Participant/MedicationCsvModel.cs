namespace Intervent.Web.DTO
{
    public class MedicationCsvModel
    {
        public string UniqueId { get; set; }

        public string Medication { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Dose { get; set; }

        public string Units { get; set; }

        public string Frequency { get; set; }
    }
}
