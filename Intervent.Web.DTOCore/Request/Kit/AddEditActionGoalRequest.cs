namespace Intervent.Web.DTO
{
    public class AddEditActionGoalRequest
    {
        public int participantId { get; set; }

        public int? kitsInUserProgramId { get; set; }

        public string goals { get; set; }

        public int? goalsId { get; set; }

        public bool achieveGoal { get; set; }
    }
}