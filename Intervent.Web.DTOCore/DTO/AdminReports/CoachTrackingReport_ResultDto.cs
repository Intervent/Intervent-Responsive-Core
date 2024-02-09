namespace Intervent.Web.DTO
{
    public class CoachTrackingReport_ResultDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string CoachName { get; set; }

        public string ProgramName { get; set; }

        public int? CompletedCalls { get; set; }

        public int? TotalCalls { get; set; }

        public bool? FollowUp { get; set; }

        public string Timezone { get; set; }

        public int? Records { get; set; }

    }
}
