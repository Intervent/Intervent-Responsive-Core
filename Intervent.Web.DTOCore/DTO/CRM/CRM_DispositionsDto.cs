namespace Intervent.Web.DTO
{
    public class CRM_DispositionsDto
    {
        public int Id { get; set; }

        public string Disposition { get; set; }

        public bool? Complaint { get; set; }

        public int CategoryId { get; set; }

        public bool EligibleforActivity { get; set; }

        public virtual IList<CRM_NoteDto> CRM_Notes { get; set; }
    }
}
