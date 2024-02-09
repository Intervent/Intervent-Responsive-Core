using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class MobileReader : BaseDataReader
    {
        InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public List<UserLoggedInDevicesDto> GetUserNotificationDevices(int userId)
        {
            var detail = context.UserLoggedInDevices.Include("User").Include("User.Country1").Where(x => x.User.Id == userId && !string.IsNullOrEmpty(x.Token)).ToList();
            return Utility.mapper.Map<List<UserLoggedInDevice>, List<UserLoggedInDevicesDto>>(detail);
        }

        public GetUserNotificationDeviceResponse CanShowMobileNotificationDetails(int userId, string deviceId)
        {
            GetUserNotificationDeviceResponse response = new GetUserNotificationDeviceResponse { canShowNotification = false, notificationEnabled = false };
            var list = context.UserLoggedInDevices.Include("User").Where(x => x.DeviceId == deviceId && !string.IsNullOrEmpty(x.Token)).ToList();
            if (list.Count() == 0)
                response.canShowNotification = true;
            if (list.Where(x => x.UserId == userId).Count() > 0)
            {
                response.notificationEnabled = true;
                response.canShowNotification = true;
            }
            return response;
        }
    }
}
