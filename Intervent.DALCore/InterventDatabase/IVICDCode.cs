using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public partial class IVICDCode
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(10)]
        public string CodeFlag { get; set; }

        [StringLength(500)]
        public string CodeDescription { get; set; }

        [StringLength(50)]
        public string CodeSource { get; set; }
    }
}
