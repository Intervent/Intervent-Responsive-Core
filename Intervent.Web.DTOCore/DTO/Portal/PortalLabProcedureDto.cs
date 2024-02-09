namespace Intervent.Web.DTO
{
    public class PortalLabProcedureDto
    {

        public int Id { get; set; }

        public int PortalId { get; set; }

        public int LabProcedureId { get; set; }

        public LabProcedureDto LabProcedure { get; set; }

        public PortalDto Portal { get; set; }
    }
}
