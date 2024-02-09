using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class ChallengeReader
    {
        InterventDatabase dbcontext = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());
        public UserActivitiesResponse GetUserActivitiesDetail(UserActivitiesRequest request)
        {
            UserActivitiesResponse response = new UserActivitiesResponse();
            var raffles = dbcontext.UserRaffles.Include("RafflesinPortal").Include("RafflesinPortal.RaffleTypes").Where(x => x.userId == request.userId && x.RafflesinPortal.PortalId == request.portalId).ToList();
            response.userRaffles = Utility.mapper.Map<List<UserRaffles>, List<UserRafflesDto>>(raffles);
            var rafflesinPortals = dbcontext.RafflesinPortals.Include("RaffleTypes").Include("RaffleDates").Where(x => x.PortalId == request.portalId && x.RaffleDates.Count() > 0 && x.isActive).OrderByDescending(x => x.RaffleTypes.Id).ToList();
            response.rafflesinPortals = Utility.mapper.Map<List<RafflesinPortals>, List<RafflesinPortalsDto>>(rafflesinPortals);
            var keys = dbcontext.UserKeys.Where(x => x.userId == request.userId).ToList();
            response.userKeys = Utility.mapper.Map<List<UserKeys>, List<UserKeysDto>>(keys);
            return response;
        }
    }
}
