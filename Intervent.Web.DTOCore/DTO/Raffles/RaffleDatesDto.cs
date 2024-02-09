namespace Intervent.Web.DTO
{
    public class RaffleDatesDto
    {
        public int Id { get; set; }

        public int RafflesinPortalsId { get; set; }

        public DateTime RaffleDate { get; set; }

        public string RaffleDateStr
        {
            get { return RaffleDate.ToShortDateString(); }
            set { }
        }
    }
}
