namespace Intervent.Web.DTO
{
    public class UnapprovedCarePlan
    {
        public int userId { get; set; }

        public string userName { get; set; }

        public int refId { get; set; }

        public int reportType { get; set; }

        public int usersinProgramId { get; set; }

        public string uniqueId { get; set; }

        public DateTime completedDate { get; set; }


    }
}