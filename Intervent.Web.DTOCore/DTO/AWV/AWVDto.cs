namespace Intervent.Web.DTO
{
    public class AWVDto
    {
        public int Id { get; set; }

        public int PortalId { get; set; }

        public int UserId { get; set; }

        public DateTime AssessmentDate { get; set; }

        public Nullable<bool> IPPE { get; set; }

        public Nullable<bool> AWV1 { get; set; }

        public Nullable<bool> SubAWV { get; set; }

        public string AWVPageSeqDone { get; set; }

        public bool GoalsGenerated { get; set; }

        public string ConductedBy { get; set; }

        public Nullable<System.DateTime> LastAssessmentDate { get; set; }

        public Nullable<System.DateTime> MedBEligDate { get; set; }

        public string ReferenceId { get; set; }

        public Nullable<System.DateTime> DateCreated { get; set; }

        public string ProviderName { get; set; }

        public string ProviderAddress { get; set; }

        public Nullable<System.DateTime> DateUpdated { get; set; }

        public string Token { get; set; }

        public string DrComments { get; set; }
    }
}
