using System.Text;

namespace Intervent.Web.DTO
{
    public sealed class BulkAddEligibilityandLogBufferResponse
    {
        public int successCount { get; set; }

        public StringBuilder captivaCSV { get; set; }

    }
}