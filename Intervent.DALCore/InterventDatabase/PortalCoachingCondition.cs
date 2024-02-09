namespace Intervent.DAL
{
    public partial class PortalCoachingConditions
    {
        public int Id { get; set; }

        public int PortalId { get; set; }

        public int CoachCondId { get; set; }

        public virtual Portal Portal { get; set; }

        public virtual CoachingConditions CoachingConditions { get; set; }
    }
}
