using Intervent.Web.DataLayer;
using System.Configuration;

namespace Intervent.Business.Organization
{
    public class PortalManager
    {
        public void DeactivateInactivePortal()
        {
            PortalReader _portalReader = new PortalReader();
            _portalReader.DeactivatePortal(Convert.ToInt32(ConfigurationManager.AppSettings["SystemAdminId"]));
        }

    }
}
