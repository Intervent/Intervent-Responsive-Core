namespace Intervent.Web.DTO
{
    public class GetBillingNotesResponse
    {
        public IList<BillingNotesDto> billingNotes { get; set; }
    }

    public class GetPreviousWellnessDataResponse
    {
        public BillingNotesDto billingNote { get; set; }
    }
}
