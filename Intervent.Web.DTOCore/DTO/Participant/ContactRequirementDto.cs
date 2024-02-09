namespace Intervent.Web.DTO
{
    public class ContactRequirementDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int? RefId { get; set; }

        public int Type { get; set; }

        public int AlertId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public virtual ContactRequirementsAlertDto ContactRequirementsAlerts { get; set; }
    }
}
