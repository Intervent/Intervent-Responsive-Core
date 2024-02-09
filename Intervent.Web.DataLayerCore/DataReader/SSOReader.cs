using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class SSOReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public IList<SSOProviderDto> GetAllProviders()
        {
            var providers = context.SSOProviders.Include("SSOAttributeMappings").ToList();
            return Utility.mapper.Map<IList<SSOProvider>, IList<SSOProviderDto>>(providers);
        }
    }
}
