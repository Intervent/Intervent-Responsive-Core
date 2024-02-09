namespace Intervent.Web.DTO
{
    public class ListKeyActionStepsResponse
    {
        public IList<ActionStep> lifetimeActionSteps { get; set; }

        public IList<ActionStep> gapActionSteps { get; set; }
    }

    public class ActionStep
    {
        public string name { get; set; }

        public bool noneIdentified { get; set; }
    }
}
