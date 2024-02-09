namespace Intervent.Web.DTO
{
    public class ProgramDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte ProgramType { get; set; }

        public byte? RiskLevel { get; set; }

        public bool Smoking { get; set; }

        public bool Pregancy { get; set; }

        public bool Active { get; set; }

        public string ImageUrl { get; set; }

        public string Code { get; set; }

        public IList<KitsinProgramDto> kitsinProgram { get; set; }
    }
}