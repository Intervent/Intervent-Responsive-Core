namespace Intervent.Web.DTO
{
    public class AddLabRequest
    {
        public int Id { get; set; }

        public int updatedBy { get; set; }

        public int HRAValidity { get; set; }

        public int? userinprogramId { get; set; }

        public LabDto Lab { get; set; }

        public bool SaveNew { get; set; }

        public bool updateLab { get; set; }

        public bool overrideCurrentValue { get; set; }

        public bool skipAssessmentUpdate { get; set; }
    }
}
