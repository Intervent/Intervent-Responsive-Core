namespace Intervent.Web.DTO
{
    public class CheckProgramConditionResponse
    {
        public bool eligibility { get; set; }

        public bool canriskSource { get; set; }

        public bool labSource { get; set; }

        public bool pending { get; set; }
    }
}
