namespace Intervent.Web.DTO
{
    public class UpdateHealthNumbersFromLabRequest
    {
        public LabDto lab { get; set; }

        public int HRAValidity { get; set; }

        public bool saveNew { get; set; }

        public int updatedBy { get; set; }

        public bool overrideCurrentValue { get; set; }
    }
}
