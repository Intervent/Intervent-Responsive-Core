namespace Intervent.Web.DTO
{
    public class SaveUserFormRequest
    {
        public int userId { get; set; }

        public int portalId { get; set; }

        public string reference { get; set; }

        public int formTypeId { get; set; }

    }
}
