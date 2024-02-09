namespace Intervent.Web.DTO
{
    public class ListUnapprovedCarePlanResponse
    {
        public List<UnapprovedCarePlan> UnapprovedCarePlans { get; set; }

        public List<int> portalIds { get; set; }

        public int totalRecords { get; set; }

    }
}