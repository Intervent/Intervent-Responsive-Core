namespace Intervent.Web.DTO
{
    public sealed class EmailRequestDto
    {
        public string FunctionalityName { get; set; }

        public string AdditionalMessage { get; set; }

        public int SuccessCount { get; set; }

        public int ErrorCount { get; set; }

        public string OrganizationName { get; set; }

        public int TerminatedRecordsCount { get; set; }

        public string DateandTime { get; set; }

    }


}
