namespace Intervent.Web.DTO
{
    public class FollowUpDto
    {
        public int Id { get; set; }
        public int UsersinProgramsId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public string PageSeqDone { get; set; }
        public int PercentComplete { get; set; }
        public int? CreatedBy { get; set; }
        public UsersinProgramDto UsersinProgram { get; set; }
    }
}
