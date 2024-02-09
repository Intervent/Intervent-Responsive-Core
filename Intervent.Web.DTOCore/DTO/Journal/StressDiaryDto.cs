namespace Intervent.Web.DTO
{
    public class StressDiaryDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public byte Duration { get; set; }

        public string Trigger { get; set; }

        public string Response { get; set; }

        public byte Intensity { get; set; }

        public bool Active { get; set; }

        public DateTime AddedOn { get; set; }
    }
}
