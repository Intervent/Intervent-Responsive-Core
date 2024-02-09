using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Intervent.DAL
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<InterventDatabase>
    {
        public InterventDatabase CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InterventDatabase>();
            optionsBuilder.UseSqlServer("data source=.;initial catalog=master;integrated security=True;MultipleActiveResultSets=True;Trusted_Connection=True;TrustServerCertificate=True");

            return new InterventDatabase(optionsBuilder.Options);
        }
    }
}
