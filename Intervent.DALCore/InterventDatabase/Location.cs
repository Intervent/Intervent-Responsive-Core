namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class Location
    {
        public int Id { get; set; }

        public string Code { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }

        public int? OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
