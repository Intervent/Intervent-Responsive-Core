using System.Data.Entity;

namespace ClaimDataAnalytics.Claims.DB
{
    public partial class AnalyticsContext : DbContext
    {
        public AnalyticsContext()
            : base("name=AnalyticsDb")
        {
        }
        public virtual DbSet<DboCrothalId> CrothalIds { get; set; }
        public virtual DbSet<DboEligibility> EligibilityIds { get; set; }
        public virtual DbSet<DboExcludeId> ExcludeIds { get; set; }
    }
}
