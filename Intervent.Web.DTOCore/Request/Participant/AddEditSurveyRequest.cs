namespace Intervent.Web.DTO
{
    public class AddEditSurveyRequest
    {
        public List<SurveyResponseDto> surveyList { get; set; }

        public bool isEligibleForIncentive { get; set; }

        public string comments { get; set; }

        public int userId { get; set; }

        public int portalId { get; set; }

    }
}
