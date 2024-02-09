namespace Intervent.Web.DTO
{
    public class CreateHRARequest
    {
        public HRADto HRA { get; set; }

        public string languageCode { get; set; }

        public int? UserinProgramId { get; set; }
    }
}