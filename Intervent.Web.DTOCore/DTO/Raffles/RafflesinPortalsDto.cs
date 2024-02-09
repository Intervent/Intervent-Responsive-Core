namespace Intervent.Web.DTO
{
    public class RafflesinPortalsDto
    {
        public int Id { get; set; }

        public int PortalId { get; set; }

        public int RaffleTypeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool isActive { get; set; }

        public Dictionary<int, DateTime?> Date { get; set; }

        public string Reward { get; set; }

        public int Points { get; set; }

        public RaffleTypesDto RaffleTypes { get; set; }

        public List<RaffleDatesDto> RaffleDates { get; set; }

        public PortalDto portal { get; set; }
    }
}
