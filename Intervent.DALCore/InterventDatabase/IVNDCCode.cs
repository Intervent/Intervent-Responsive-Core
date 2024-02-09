using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public partial class IVNDCCode
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(10)]
        public string CodeFlag { get; set; }

        [StringLength(255)]
        public string CodeDescription { get; set; }
    }
}
