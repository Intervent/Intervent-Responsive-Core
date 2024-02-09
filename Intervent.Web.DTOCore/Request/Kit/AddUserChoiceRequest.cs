namespace Intervent.Web.DTO
{
    public class AddUserChoiceRequest
    {
        public KeyValueDTO[] UserAnswer { get; set; }

        public int KitsInUserProgramsId { get; set; }

        public int PercentComplete { get; set; }

        public bool IsQuiz { get; set; }

        public int UpdatedBy { get; set; }

        public bool IsIntuityUser { get; set; }

        public int ParticipantId { get; set; }

        public int IntegrationWith { get; set; }
    }
}
