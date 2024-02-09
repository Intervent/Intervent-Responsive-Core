using System.ComponentModel.DataAnnotations;

namespace Intervent.Web.DTO
{
    public class IntuityActivityCsvModel
    {

        [MaxLength(100)]
        public string UniqueID { get; set; }

        public string EventType { get; set; }

        public DateTime? DateTimeStamp { get; set; }

    }
}
