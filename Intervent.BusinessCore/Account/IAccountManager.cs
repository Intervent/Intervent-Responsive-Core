using Intervent.Web.DTO;

namespace Intervent.Business.Account
{
    public interface IAccountManager
    {
        ListPortalsResponse ListPortals(int? OrganizationId);

        GetCurrentPortalIdResponse CurrentPortalId(int OrganizationId);

        FindUsersResponse ListUsersWithValidEmailforOrganization(IEnumerable<int> organizationIds);

        List<int> GetOrganizationToProcessClaims();
    }
}
