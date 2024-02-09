namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class CouponCode
    {
        public int Id { get; set; }

        public int? OrganizationId { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(500)]
        public string? Name { get; set; }

        public bool? Active { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
