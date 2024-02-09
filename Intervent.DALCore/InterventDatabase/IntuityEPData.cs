using System.ComponentModel.DataAnnotations.Schema;

namespace Intervent.DAL
{
    [Table("IntuityEPData")]
    public partial class IntuityEPData
    {
        public int Id { get; set; }

        public int IntuityEligibilityId { get; set; }

        public int? PatternsRegCount { get; set; }

        public int? SyncTestResultCount { get; set; }

        public int? PairMonitorCount { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public virtual IntuityEligibility IntuityEligibility { get; set; }
    }
}
