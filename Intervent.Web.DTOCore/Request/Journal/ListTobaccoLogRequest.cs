﻿namespace Intervent.Web.DTO
{
    public class ListTobaccoLogRequest
    {
        public int userId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int? TotalRecords { get; set; }

    }
}