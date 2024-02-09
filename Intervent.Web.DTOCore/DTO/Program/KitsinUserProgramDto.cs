namespace Intervent.Web.DTO
{
    public class KitsinUserProgramDto
    {
        public int Id { get; set; }

        public int UsersinProgramsId { get; set; }

        public int KitId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        public int PercentCompleted { get; set; }

        public bool IsActive { get; set; }

        public bool ListenedAudio { get; set; }

        public KitsDto Kit { get; set; }

        public IList<KitsinUserProgramGoalDto> KitsinUserProgramGoals { get; set; }
    }
}
