using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public class RaffleTypes
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(10)]
        public string? LangItemName { get; set; }

        public ICollection<RafflesinPortals> RafflesinPortals { get; set; }
    }
}
