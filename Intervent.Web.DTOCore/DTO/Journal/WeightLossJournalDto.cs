namespace Intervent.Web.DTO
{
    public class WeightLossJournalDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public string DateString
        {
            get
            {
                return Date.ToShortDateString();
            }
        }

        public int DayNo { get; set; }

        public float? Waist { get; set; }

        public string WaistText { get; set; }

        public float? Weight { get; set; }

        public string WeightText { get; set; }

        public string Food { get; set; }

        public string NotAuthorizedFood { get; set; }

        public bool HadWater { get; set; }

        public bool CutSodium { get; set; }

        public bool SideEffects { get; set; }

        public byte? MotivationScale { get; set; }

        public int? Exercise { get; set; }

        public string Activity { get; set; }

        public string Comments { get; set; }

        public int UpdatedBy { get; set; }
    }
}
