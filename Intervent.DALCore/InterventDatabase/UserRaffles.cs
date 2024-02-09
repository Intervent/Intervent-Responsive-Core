using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public class UserRaffles
    {
        [Key]
        public int Id { get; set; }

        public int userId { get; set; }

        public int RafflePortalId { get; set; }

        public DateTime? Date { get; set; }

        public virtual User user { get; set; }

        public virtual RafflesinPortals RafflesinPortal { get; set; }
    }
}
