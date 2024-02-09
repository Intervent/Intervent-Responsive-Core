using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public class RaffleDates
    {
        [Key]
        public int Id { get; set; }

        public int RafflesinPortalsId { get; set; }

        public DateTime RaffleDate { get; set; }

        public virtual RafflesinPortals RafflesinPortals { get; set; }
    }
}
