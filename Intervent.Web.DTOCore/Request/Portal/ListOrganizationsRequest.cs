namespace Intervent.Web.DTO
{
    public class ListOrganizationsRequest
    {
        public bool onlyActive { get; set; }

        public bool removechildOrganizations { get; set; }

        public bool includeParentOrganization { get; set; }

        public int? ParentOrganizationId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int? TotalRecords { get; set; }

        public int? FilterBy { get; set; }

        public string Search { get; set; }

    }
}