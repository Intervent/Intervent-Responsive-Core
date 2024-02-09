namespace Intervent.Web.DTO
{
    public class GetProgramsByPortalRequest
    {
        public int PortalId { get; set; }

        public int? ProgramType { get; set; }

        public bool? onlyActive { get; set; }
    }
}