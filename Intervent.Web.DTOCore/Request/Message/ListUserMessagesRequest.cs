namespace Intervent.Web.DTO
{
    public class ListUserMessagesRequest
    {
        public int userId { get; set; }

        public string timeZone { get; set; }

        public bool isAdmin { get; set; }

        public bool allMessages { get; set; }

        public int? coachId { get; set; }

        public bool drafts { get; set; }

        public bool getCount { get; set; }

        public string searchText { get; set; }

        public bool unread { get; set; }

        public bool hasPagination { get; set; }

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

        public int adminId { get; set; }

        public int systemAdminId { get; set; }

    }
}
