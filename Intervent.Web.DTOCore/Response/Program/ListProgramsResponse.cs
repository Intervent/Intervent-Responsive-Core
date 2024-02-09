namespace Intervent.Web.DTO
{
    public class ListProgramsResponse
    {
        public IList<ProgramDto> Programs { get; set; }

        public int totalRecords { get; set; }
    }
}