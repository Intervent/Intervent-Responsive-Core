namespace Intervent.Web.DTO
{
    public class KitsinUserProgramGoalDto
    {
        public int Id { get; set; }

        public int KitsinUserProgramId { get; set; }

        public string Goals { get; set; }

        public bool GoalsAchieved { get; set; }

        public DateTime? DateAchieved { get; set; }

        //public virtual KitsinUserProgramDto KitsinUserProgram { get; set; }
    }
}
