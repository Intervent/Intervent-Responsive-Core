namespace Intervent.Web.DTO
{
    public class CRM_ChangeLogDto
    {
        public long Id { get; set; }

        public DateTime LogDate { get; set; }

        public int ContactId { get; set; }

        public int? RefId { get; set; }

        public string Changes { get; set; }

        public int CategoryId { get; set; }

        public int UpdatedBy { get; set; }

        public UserDto User { get; set; }

        public virtual CRM_ContactDto CRM_Contacts { get; set; }

    }
}
