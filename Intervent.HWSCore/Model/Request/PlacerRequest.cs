namespace Intervent.HWS
{
    public class PlacerRequest
    {
        public class GetExternalReportsList
        {
            public string uniqueId { get; set; }
            public string tenant { get; set; }
        }

        public class GetExternalReports
        {
            public string uniqueId { get; set; }
            public string reportName { get; set; }
            public string tenant { get; set; }
        }

        public class CoachingNotes
        {
            public string study_subject_ID { get; set; }
            public string tenant { get; set; }
            public DateTime date { get; set; }
            public string coach_name { get; set; }
            public string notes { get; set; }
            public string duration_minutes { get; set; }
        }


        public class GetRPMSummaryGraph
        {
            public string uniqueId { get; set; }
            public DateTime startDate { get; set; }
            public DateTime endDate { get; set; }
        }
    }
}
