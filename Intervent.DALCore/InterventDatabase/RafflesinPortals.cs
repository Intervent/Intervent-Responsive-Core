using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public class RafflesinPortals
    {
        [Key]
        public int Id { get; set; }

        public int PortalId { get; set; }

        public int RaffleTypeId { get; set; }

        public string? Reward { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        public int Points { get; set; }

        public bool isActive { get; set; }

        public virtual RaffleTypes RaffleTypes { get; set; }

        public virtual Portal portal { get; set; }

        public ICollection<UserRaffles> UserRaffles { get; set; }

        public ICollection<RaffleDates> RaffleDates { get; set; }
    }
}

