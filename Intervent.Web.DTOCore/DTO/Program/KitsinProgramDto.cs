namespace Intervent.Web.DTO
{
    public class KitsinProgramDto
    {
        public int ProgramId { get; set; }

        public int KitId { get; set; }

        public short Order { get; set; }

        public bool Active { get; set; }

        public KitsDto eduKit { get; set; }

        public ProgramDto program { get; set; }
    }
}