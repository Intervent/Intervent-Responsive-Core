namespace Intervent.Web.DTO
{
    public class ProcessBillingNotesRequest
    {
        public int orgId { get; set; }
    }

    public class EditBillingNotesRequest
    {
        public BillingNotesDto BillingNote { get; set; }
    }
}