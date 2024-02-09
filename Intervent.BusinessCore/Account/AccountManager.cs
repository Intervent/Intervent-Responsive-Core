using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace Intervent.Business.Account
{
    public class AccountManager : IAccountManager
    {
        PortalReader _portalReader = new PortalReader();
        AccountReader _accountReader = new AccountReader();

        public ListPortalsResponse ListPortals(int? OrganizationId)
        {
            ListPortalsRequest request = new ListPortalsRequest();
            if (OrganizationId.HasValue)
                request.organizationId = OrganizationId.Value;
            request.onlyActive = true;
            return _portalReader.ListPortals(request);
        }


        public GetCurrentPortalIdResponse CurrentPortalId(int OrganizationId)
        {
            ListPortalsRequest request = new ListPortalsRequest();
            request.organizationId = OrganizationId;
            return _portalReader.CurrentPortalIdForOrganization(request);
        }

        public FindUsersResponse ListUsersWithValidEmailforOrganization(IEnumerable<int> organizationIds)
        {
            var response = _accountReader.FindUsers(new FindUsersRequest() { OrganizationIds = organizationIds });
            response.Users = response.Users.Where(x => (x.Email != null && x.Email != "")).ToList();
            return response;
        }

        public List<int> GetOrganizationToProcessClaims()
        {
            var response = _portalReader.GetOrganizationToProcessClaims();
            return response;
        }

        public void DeleteOldPictures()
        {
            var path = ""; //add file path.
            var fileNames = Directory.GetFiles(path);
            var picturesList = new ParticipantReader().GetProfilePictures();
            foreach (var file in fileNames)
            {
                var filename = Path.GetFileName(file);
                if (!picturesList.Contains(filename))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
