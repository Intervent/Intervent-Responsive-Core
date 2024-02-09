namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OrganizationsforWebinar")]
    public partial class OrganizationsforWebinar
    {
        public int Id { get; set; }

        public int WebinarId { get; set; }

        public int OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual Webinar Webinar { get; set; }
    }
}
