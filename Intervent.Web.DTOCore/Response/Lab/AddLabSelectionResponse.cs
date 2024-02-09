namespace Intervent.Web.DTO
{
    public class AddLabSelectionResponse
    {
        public bool status { get; set; }

        public bool duplicate { get; set; }

        public string OrderId { get; set; }

        public int labId { get; set; }

        public int labSelection { get; set; }
    }
}
