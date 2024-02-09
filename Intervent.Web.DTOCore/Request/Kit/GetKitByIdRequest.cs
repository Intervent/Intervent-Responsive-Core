namespace Intervent.Web.DTO
{
    public class GetKitByIdRequest
    {
        public int kitId { get; set; }

        public int? kitsInUserProgramId { get; set; }

        public int userinProgramId { get; set; }

        public bool? preview { get; set; }

        public string PreviewLanguage { get; set; }

    }
}
