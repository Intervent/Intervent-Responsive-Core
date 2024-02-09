namespace Intervent.Web.DTO
{
    public class SurveyResponseDto
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public int UsersinProgramsId { get; set; }

        public int Answer { get; set; }

        public DateTime? DateCreated { get; set; }

        public UsersinProgramDto UsersinProgram { get; set; }

    }
}
