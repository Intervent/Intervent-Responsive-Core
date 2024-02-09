namespace Intervent.DAL
{
    using System;

    public partial class KitsinUserProgramGoal
    {
        public int Id { get; set; }

        public int KitsinUserProgramId { get; set; }

        public string? Goals { get; set; }

        public bool GoalsAchieved { get; set; }

        public DateTime? DateAchieved { get; set; }

        public virtual KitsinUserProgram KitsinUserProgram { get; set; }
    }
}
