using Intervent.DAL;

namespace Intervent.Web.DTO
{
    public class BulkUpdateIntuityEligibilityLogRequest
    {
        public IList<IntuityEligibilityLog> IntuityEligibilityLogList { get; set; }
    }
}
