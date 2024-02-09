namespace Intervent.Web.DTO
{
    public class GetStepWithActivityRequest
    {
        public int stepId { get; set; }

        public List<int> activityIds { get; set; }

        public string languageCode { get; set; }
    }
}
