namespace Intervent.Web.DTO
{
    public class GetPregnencyStatusResponse
    {
        public bool isPregnant { get; set; }

        public DateTime? pregDueDate { get; set; }

        public string Trimester { get; set; }
    }
}
