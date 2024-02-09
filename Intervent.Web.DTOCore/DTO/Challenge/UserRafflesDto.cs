namespace Intervent.Web.DTO
{
    public class UserRafflesDto
    {
        public int Id { get; set; }

        public int userId { get; set; }

        public int RafflePortalId { get; set; }

        public DateTime? Date { get; set; }

        public UserDto user { get; set; }

        public RafflesinPortalsDto RafflesinPortal { get; set; }
    }
}
