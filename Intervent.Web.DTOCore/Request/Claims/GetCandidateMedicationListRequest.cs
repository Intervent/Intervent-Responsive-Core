namespace Intervent.Web.DTO
{
    public class GetCandidateMedicationListRequest
    {
        public string UniqueId { get; set; }
        public int Organizationid { get; set; }
    }

    public class GetCandidateConditionListRequest
    {
        public string UniqueId { get; set; }

        public int Organizationid { get; set; }
    }


}
