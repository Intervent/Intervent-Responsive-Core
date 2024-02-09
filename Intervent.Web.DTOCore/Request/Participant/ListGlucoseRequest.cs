namespace Intervent.Web.DTO
{
    public class ListGlucoseRequest
    {
        public string uniqueId { get; set; }

        public int organizationId { get; set; }

        public bool onlyValidData { get; set; }

    }
}
