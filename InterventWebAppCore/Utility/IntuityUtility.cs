using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class IntuityUtility
    {
        public static GetIntuityResponse GetIntuityDetails(string uniqueId, int organizationid, int portalId)
        {
            IntuityReader reader = new IntuityReader();
            GetIntuityRequest request = new GetIntuityRequest() { UniqueId = uniqueId, OrganizationId = organizationid, PortalId = portalId };
            return reader.GetIntuityDetails(request);
        }

        public static GetUserDetailsResponse GetUserDetails(string uniqueId, int organizationid)
        {
            IntuityReader reader = new IntuityReader();
            GetUserDetailsRequest request = new GetUserDetailsRequest() { UniqueId = uniqueId, OrganizationId = organizationid };
            return reader.GetUserDetails(request);
        }

        public static UpdateIntuityResponse UpdateIntuityEligibility(IntuityEligibilityModel model, int userId)
        {
            IntuityReader reader = new IntuityReader();
            model.IntuityEligibilityLog.Height = model.HeightFeet * 12;
            if (model.HeightInch.HasValue && model.HeightInch.Value > 0)
                model.IntuityEligibilityLog.Height += model.HeightInch.Value;
            UpdateIntuityRequest request = new UpdateIntuityRequest() { IntuityEligibilityLog = model.IntuityEligibilityLog, UpdateProfile = !model.ShowNewForm, PortalId = model.PortalId };
            request.UserId = model.UpdatedBy != 0 ? model.UpdatedBy : userId;
            return reader.UpdateIntuityEligibility(request);
        }

        public static bool UpdateEligiblityStatus(string unique_id, int organizationid, int eligiblityStatus, int eligiblityReason, int userId)
        {
            IntuityReader reader = new IntuityReader();
            UpdateEligiblityStatusRequest request = new UpdateEligiblityStatusRequest
            {
                UniqueId = unique_id,
                OrganizationId = organizationid,
                EligiblityStatus = eligiblityStatus,
                EligiblityReason = eligiblityReason,
                CreatedBy = userId
            };
            return reader.UpdateEligiblityStatus(request).Status;
        }

        public static UpdateIntuityFulfillmentResponse UpdateIntuityFulfillmentRequest(int intuityEligibilityId, int quantity, string reason, bool sendMeter, int userId)
        {
            IntuityReader reader = new IntuityReader();
            UpdateIntuityFulfillmentRequest request = new UpdateIntuityFulfillmentRequest
            {
                IntuityEligibilityId = intuityEligibilityId,
                Quantity = quantity,
                numberofshipments = 1,//Default 1
                Reason = reason,
                Submitted = false,
                SendMeter = sendMeter,
                UserId = userId
            };
            return reader.UpdateIntuityFulfillmentRequest(request);
        }

        public static bool UpdateIntuityQuantityonHandRequest(int intuityEligibilityId, int quantity, int userId)
        {
            IntuityReader reader = new IntuityReader();
            UpdateIntuityQuantityOnHandRequest request = new UpdateIntuityQuantityOnHandRequest
            {
                UserId = userId,
                IntuityEligibilityId = intuityEligibilityId,
                Quantity = quantity,
                Submitted = false
            };
            return reader.UpdateIntuityQuantityonHand(request).Status;
        }

        public static UpdateIntuityOptingOutResponse UpdateIntuityOptingOut(IntuityEligibilityModel model, bool OptingOut, int userId)
        {
            IntuityReader reader = new IntuityReader();
            UpdateIntuityOptingOutRequest request = new UpdateIntuityOptingOutRequest
            {
                Status = false,
                IntuityEligibilityLog = model.IntuityEligibilityLog,
                UserId = userId,
                OptedOutDate = DateTime.UtcNow,
                OptingOut = OptingOut,
            };
            return reader.UpdateIntuityOptingOutRequest(request);
        }
    }
}