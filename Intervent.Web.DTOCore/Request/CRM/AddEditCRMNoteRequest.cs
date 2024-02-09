namespace Intervent.Web.DTO
{
    public class AddEditCRMNoteRequest
    {
        public CRM_NoteDto crm_Note { get; set; }

        public int userId { get; set; }

        public int intuityOrgId { get; set; }
    }
}
