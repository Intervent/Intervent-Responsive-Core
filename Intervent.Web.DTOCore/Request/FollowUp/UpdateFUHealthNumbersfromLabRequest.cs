namespace Intervent.Web.DTO
{
    public class UpdateFUHealthNumbersfromLabRequest
    {
        public LabDto Lab { get; set; }

        public int UsersInProgramsId { get; set; }

        public int updatedBy { get; set; }

        public bool doNotOverrideLab { get; set; }
    }
}
