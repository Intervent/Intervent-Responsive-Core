namespace Intervent.Web.DTO
{
    public class HRA_ActionStepsDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int HRAId { get; set; }

        public float Score { get; set; }

        public int Type { get; set; }

        public ActionStepTypeDto ActionStepType { get; set; }
    }
}
