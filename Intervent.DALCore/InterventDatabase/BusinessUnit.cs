using System.ComponentModel.DataAnnotations;
namespace Intervent.DAL
{
    public partial class BusinessUnit
    {
        public int Id { get; set; }

        [StringLength(25)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
    }
}
