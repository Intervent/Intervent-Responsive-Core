namespace Intervent.Web.DTO
{
    public class AddEditWellnessDataRequest
    {
        public WellnessDataDto WellnessData { get; set; }

        public int? HRAId { get; set; }

        public int? FollowUpId { get; set; }

        public bool updatedbyUser { get; set; }

        public bool IsTeamsBP { get; set; }
    }

    public class AddEditTeamsBP_PPRRequest
    {
        public TeamsBP_PPRDto TeamsBP_PPR { get; set; }
    }
}