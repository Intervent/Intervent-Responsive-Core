namespace Intervent.Web.DTO
{
    public class AssignKitsToUserProgramRequest
    {
        public IList<int> kitIds { get; set; }

        public int userId { get; set; }

        public int usersinProgramsId { get; set; }

        public byte programType { get; set; }

        public string language { get; set; }

    }
}
