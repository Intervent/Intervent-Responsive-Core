using Intervent.HWS;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class WearableUtility
    {
        public static bool AddOrEditWearableDevice(int userId, string externalUserId, string token, string refreshToken, int deviceType, bool isActive, string OauthTokenSecret, string deviceId = null, int? OffsetFromUTC = null, string scope = null)
        {
            WearableReader reader = new WearableReader();
            AddOrEditWearableDeviceRequest request = new AddOrEditWearableDeviceRequest
            {
                token = token,
                refreshToken = refreshToken,
                wearableDeviceId = deviceType,
                externalUserId = externalUserId,
                isActive = isActive,
                userId = userId,
                oauthTokenSecret = OauthTokenSecret,
                offsetFromUTC = OffsetFromUTC,
                deviceId = deviceId,
                scope = scope
            };
            return reader.AddOrEditWearableDevice(request);
        }

        public static IList<WearableDeviceDto> GetWearableDevices(int? type)
        {
            WearableReader reader = new WearableReader();
            return reader.GetWearableDevices(type);
        }

        public static IList<UserWearableDeviceDto> GetUserWearableDevices(int userId)
        {
            WearableReader reader = new WearableReader();
            return reader.GetUserWearableDevices(userId);
        }

        public static UserWearableDeviceDto GetUserWearableDevicesById(int deviceId)
        {
            WearableReader reader = new WearableReader();
            return reader.GetUserWearableDevicesById(deviceId);
        }

        public static UserWearableDeviceDto GetUserWearableDevicesByExternalUserId(string externalUserId)
        {
            WearableReader reader = new WearableReader();
            return reader.GetUserWearableDevicesByExternalUserId(externalUserId);
        }
    }
}
