using Intervent.DAL;
using Intervent.HWS;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Intervent.Web.DataLayer
{
    public class WearableReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public bool AddOrEditWearableDevice(AddOrEditWearableDeviceRequest request)
        {
            var device = context.UserWearableDevices.Where(x => x.UserId == request.userId && x.WearableDeviceId == request.wearableDeviceId && x.ExternalUserId == request.externalUserId).FirstOrDefault();
            if (device == null)
            {
                device = new UserWearableDevice();
                device.DeviceId = request.deviceId;
                device.WearableDeviceId = request.wearableDeviceId;
                device.ExternalUserId = request.externalUserId;
                device.IsActive = request.isActive;
                device.RefreshToken = request.refreshToken;
                device.Token = request.token;
                device.UserId = request.userId;
                device.CreatedOn = DateTime.UtcNow;
                device.OauthTokenSecret = request.oauthTokenSecret;
                device.OffsetFromUTC = request.offsetFromUTC;
                device.Scope = request.scope;
                context.UserWearableDevices.Add(device);
            }
            else
            {
                if (!string.IsNullOrEmpty(request.token))
                    device.Token = request.token;
                if (!string.IsNullOrEmpty(request.refreshToken))
                    device.RefreshToken = request.refreshToken;
                if (!string.IsNullOrEmpty(request.oauthTokenSecret))
                    device.OauthTokenSecret = request.oauthTokenSecret;
                if (request.offsetFromUTC.HasValue)
                    device.OffsetFromUTC = request.offsetFromUTC;
                if (!string.IsNullOrEmpty(request.scope))
                    device.Scope = request.scope;
                device.IsActive = request.isActive;
                device.UpdatedOn = DateTime.UtcNow;
                context.UserWearableDevices.Attach(device);
                context.Entry(device).State = EntityState.Modified;
            }
            context.SaveChanges();
            return true;
        }

        public IList<UserWearableDeviceDto> GetAllActiveUserWearableDevices(int wearableDeviceId)
        {
            var devices = context.UserWearableDevices.Include("User").Include("WearableDevice").Where(x => x.WearableDeviceId == wearableDeviceId && x.IsActive).ToList();
            return Utility.mapper.Map<IList<DAL.UserWearableDevice>, IList<UserWearableDeviceDto>>(devices);
        }

        public IList<WearableDeviceDto> GetWearableDevices(int? type)
        {
            var deviceTypes = context.WearableDevices.Where(x => x.IsActive && !type.HasValue || x.Type == type).ToList();
            return Utility.mapper.Map<IList<DAL.WearableDevice>, IList<WearableDeviceDto>>(deviceTypes);
        }

        public IList<UserWearableDeviceDto> GetUserWearableDevices(int userId)
        {
            var devices = context.UserWearableDevices.Include("WearableDevice").Where(x => x.UserId == userId && x.IsActive && x.WearableDevice.IsActive).ToList();
            return Utility.mapper.Map<IList<DAL.UserWearableDevice>, IList<UserWearableDeviceDto>>(devices);
        }

        public UserWearableDeviceDto GetUserWearableDevicesById(int deviceId)
        {
            var device = context.UserWearableDevices.Include("WearableDevice").Where(x => x.Id == deviceId).FirstOrDefault();
            return Utility.mapper.Map<DAL.UserWearableDevice, UserWearableDeviceDto>(device);
        }

        public UserWearableDeviceDto GetUserWearableDevicesByExternalUserId(string externalUserId)
        {
            var device = context.UserWearableDevices.Include("WearableDevice").Where(x => x.ExternalUserId == externalUserId && x.IsActive).FirstOrDefault();
            return Utility.mapper.Map<DAL.UserWearableDevice, UserWearableDeviceDto>(device);
        }
    }
}
