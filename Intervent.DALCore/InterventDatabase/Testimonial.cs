namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class Testimonial
    {
        public int Id { get; set; }

        public string? Feedback { get; set; }

        [StringLength(50)]
        public string? SignedName { get; set; }

        [StringLength(10)]
        public string? Date { get; set; }

        public int UserId { get; set; }

        public int PortalId { get; set; }
    }
}
