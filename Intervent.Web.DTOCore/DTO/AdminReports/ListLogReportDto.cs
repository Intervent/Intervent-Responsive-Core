namespace Intervent.Web.DTO
{
    public class ListLogReportDto
    {
        public int Id { get; set; }
        public DateTime? timestamp { get; set; }
        public string Level { get; set; }
        public string logger { get; set; }
        public string ExceptionType { get; set; }
        public string Message { get; set; }
        public string Operation { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
    }
}
