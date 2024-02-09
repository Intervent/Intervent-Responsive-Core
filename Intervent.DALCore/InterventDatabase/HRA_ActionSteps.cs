namespace Intervent.DAL
{
    public partial class HRA_ActionSteps
    {
        public int Id { get; set; }

        public int HRAId { get; set; }

        public float Score { get; set; }

        public int Type { get; set; }

        public bool IsNull { get; set; }

        public virtual ActionStepType ActionStepType { get; set; }

        public virtual HRA HRA { get; set; }
    }
}
