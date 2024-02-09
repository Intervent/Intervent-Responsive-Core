using Intervent.Web.DataLayer;
using System.Configuration;

namespace Intervent.Business.Organization
{
    public class PortalManager : BaseManager
    {
        public void DeactivateInactivePortal()
        {
            PortalReader _portalReader = new PortalReader();
            _portalReader.DeactivatePortal(SystemAdminId);
        }

    }
}
