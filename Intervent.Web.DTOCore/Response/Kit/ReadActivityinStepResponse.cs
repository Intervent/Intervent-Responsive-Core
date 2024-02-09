namespace Intervent.Web.DTO
{
    public class ReadActivityinStepResponse
    {
        public ActivitiesinStepsDto Activity { get; set; }

        public IList<PassiveQuestionsInActivitiesDto> PassiveQuestions { get; set; }
    }
}
