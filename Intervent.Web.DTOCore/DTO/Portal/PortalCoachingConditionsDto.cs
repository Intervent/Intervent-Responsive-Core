namespace Intervent.Web.DTO
{
    public class PortalCoachingConditionsDto
    {
        public int Id { get; set; }

        public int PortalId { get; set; }

        public int CoachCondId { get; set; }

        public virtual PortalDto Portal { get; set; }

        public virtual CoachingConditionsDto CoachingConditions { get; set; }
    }
}
