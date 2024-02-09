namespace Intervent.HWS
{
    public class PullReportListResponse : ProcessResponse
    {
        public PullReportList ParticipantReportList { get; set; }

        public ExternalReport ExternalReportData { get; set; }

        public RPMGraph RPMGraphData { get; set; }

        public class PullReportList
        {
            public string tenant { get; set; }
            public string study_subject_ID { get; set; }
            public List<string> ReportList { get; set; }
        }

        public class ExternalReport
        {
            public byte[] ReportData { get; set; }
        }

        public class RPMGraph
        {
            public byte[] GraphData { get; set; }
        }


    }
}
