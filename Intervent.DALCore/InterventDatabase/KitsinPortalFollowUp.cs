using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Intervent.DAL
{
    public partial class KitsinPortalFollowUp
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PortalFollowUpId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int KitId { get; set; }

        public short Order { get; set; }

        public bool Active { get; set; }

        public virtual Kit Kit { get; set; }

        public virtual PortalFollowUp PortalFollowUp { get; set; }
    }
}
