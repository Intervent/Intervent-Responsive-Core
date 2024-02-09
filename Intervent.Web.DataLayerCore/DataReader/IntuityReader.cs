using Intervent.DAL;
using Intervent.HWS;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Intervent.Web.DataLayer
{
    public class IntuityReader : BaseDataReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public GetIntuityResponse GetIntuityDetails(GetIntuityRequest request)
        {
            GetIntuityResponse response = new GetIntuityResponse();
            var intuityEligibilityLogs = context.IntuityEligibilityLogs.Where(x => x.UniqueId == request.UniqueId && x.OrganizationId == request.OrganizationId && x.HasDiabetes.HasValue).OrderByDescending(x => x.DateCreated).ToList();
            if (intuityEligibilityLogs.Count > 0)
                response.IntuityEligibilityLog = Utility.mapper.Map<DAL.IntuityEligibilityLog, IntuityEligibilityLogDto>(intuityEligibilityLogs.FirstOrDefault());
            var intuityEligibility = context.IntuityEligibilities.Include("IntuityFulfillments").Include("IntuityFulfillmentRequests").Include("IntuityQOHs").Where(x => x.UniqueId == request.UniqueId && x.OrganizationId == request.OrganizationId).OrderByDescending(x => x.DateCreated).FirstOrDefault();
            if (intuityEligibility != null)
            {
                response.IntuityEligibility = IntuityEligibilityDto.MapToIntuityEligibilityDto(intuityEligibility);
                response.UserEligible = response.IntuityEligibility.EligibilityStatus == EligibilityStatus.Eligible;
                if (response.IntuityEligibilityLog == null)
                {
                    response.ShowNewForm = true;
                    CommonReader _commonReader = new CommonReader();
                    var States = _commonReader.ListAllStates().States;
                    var Countries = _commonReader.ListCountries(new ListCountriesRequest()).Countries;
                    var eligibility = context.Eligibilities.Where(x => x.UniqueId == request.UniqueId && x.PortalId == request.PortalId).FirstOrDefault();
                    response.IntuityEligibilityLog = new IntuityEligibilityLogDto();
                    response.IntuityEligibilityLog.OptingOut = intuityEligibility.OptingOut;
                    if (eligibility != null)
                    {
                        response.IntuityEligibilityLog.FirstName = eligibility.FirstName;
                        response.IntuityEligibilityLog.LastName = eligibility.LastName;
                        response.IntuityEligibilityLog.email = eligibility.Email;
                        response.IntuityEligibilityLog.PhoneNumber = !string.IsNullOrEmpty(eligibility.HomeNumber) ? eligibility.HomeNumber : !string.IsNullOrEmpty(eligibility.CellNumber) ? eligibility.CellNumber : eligibility.WorkNumber;
                        response.IntuityEligibilityLog.AddressLine1 = eligibility.Address;
                        response.IntuityEligibilityLog.AddressLine2 = eligibility.Address2;
                        response.IntuityEligibilityLog.City = eligibility.City;
                        response.IntuityEligibilityLog.State = States.Where(x => x.Code == eligibility.State && x.CountryId == Countries.Where(c => c.UNCode == eligibility.Country).Select(c => c.Id).FirstOrDefault()).Select(x => x.Id).FirstOrDefault();
                        response.IntuityEligibilityLog.Country = Countries.Where(c => c.UNCode == eligibility.Country).Select(c => c.Id).FirstOrDefault();
                        response.IntuityEligibilityLog.Zip = eligibility.Zip;
                        response.IntuityEligibilityLog.EligibilityId = eligibility.Id;
                    }
                }
                else
                {
                    response.IntuityEligibilityLog.EligibilityStatus = intuityEligibility.EligibilityStatus;
                    response.IntuityEligibilityLog.EligibilityReason = intuityEligibility.EligibilityReason;
                    response.IntuityEligibilityLog.FirstName = intuityEligibility.FirstName;
                    response.IntuityEligibilityLog.LastName = intuityEligibility.LastName;
                    response.IntuityEligibilityLog.email = intuityEligibility.email;
                    response.IntuityEligibilityLog.PhoneNumber = intuityEligibility.PhoneNumber;
                    response.IntuityEligibilityLog.AddressLine1 = intuityEligibility.AddressLine1;
                    response.IntuityEligibilityLog.AddressLine2 = intuityEligibility.AddressLine2;
                    response.IntuityEligibilityLog.City = intuityEligibility.City;
                    response.IntuityEligibilityLog.State = intuityEligibility.State;
                    response.IntuityEligibilityLog.Country = intuityEligibility.Country;
                    response.IntuityEligibilityLog.Zip = intuityEligibility.Zip;
                    response.IntuityEligibilityLog.OptingOut = intuityEligibility.OptingOut;
                    response.IntuityEligibilityId = intuityEligibility.Id;
                    response.OverrideStatus = intuityEligibility.OverrideStatus ?? false;
                    response.FormSubmittedDate = intuityEligibilityLogs.Where(x => x.DateCreated.HasValue).Count() > 0 ? intuityEligibilityLogs.Where(x => x.DateCreated.HasValue).OrderBy(x => x.Id).FirstOrDefault().DateCreated : null;
                    response.PatternsRegDate = intuityEligibilityLogs.Where(x => x.PatternsRegDate.HasValue).Count() > 0 ? intuityEligibilityLogs.Where(x => x.PatternsRegDate.HasValue).OrderBy(x => x.Id).FirstOrDefault().PatternsRegDate : null;
                    response.IntuityFulfillments = Utility.mapper.Map<IList<DAL.IntuityFulfillments>, IList<IntuityFulfillmentsDto>>(intuityEligibility.IntuityFulfillments.ToList());
                    response.IntuityFulfillmentRequests = Utility.mapper.Map<IList<DAL.IntuityFulfillmentRequests>, IList<IntuityFulfillmentRequestsDto>>(intuityEligibility.IntuityFulfillmentRequests.OrderByDescending(x => x.CreatedOn).ToList());
                    response.IntuityQOH = Utility.mapper.Map<IList<DAL.IntuityQOH>, IList<IntuityQOHDto>>(intuityEligibility.IntuityQOHs.OrderByDescending(x => x.CreatedOn).ToList());
                    response.ShowFulfillment = response.IntuityFulfillmentRequests.Count() > 0 && response.IntuityFulfillmentRequests.Any(x => x.CreatedOn.Date == DateTime.UtcNow.Date) ? false : true;
                }
            }
            return response;
        }

        public GetUserDetailsResponse GetUserDetails(GetUserDetailsRequest request)
        {
            GetUserDetailsResponse response = new GetUserDetailsResponse();
            var user = context.Users.Where(x => x.UniqueId == request.UniqueId && x.OrganizationId == request.OrganizationId).FirstOrDefault();
            response.User = Utility.mapper.Map<User, UserDto>(user);
            return response;
        }

        public bool HasIntuityEligibilityRecord(GetIntuityRequest request)
        {
            return context.IntuityEligibilities.Where(i => i.OrganizationId == request.OrganizationId && i.UniqueId == request.UniqueId).Count() > 0;
        }

        public GetIntuityFulfillmentRequestsResponse GetIntuityFulfillmentRequestsDetails(IntuityFulfillmentRequests request)
        {
            GetIntuityFulfillmentRequestsResponse response = new GetIntuityFulfillmentRequestsResponse();
            var intuityFulfillmentRequests = context.IntuityFulfillmentRequests.Where(x => x.IntuityEligibilityId == request.IntuityEligibilityId).OrderByDescending(x => x.CreatedOn).ToList();
            response.IntuityFulfillmentRequests = Utility.mapper.Map<IList<DAL.IntuityFulfillmentRequests>, IList<IntuityFulfillmentRequestsDto>>(intuityFulfillmentRequests);
            return response;
        }

        public GetIntuityFulfillmentRequestsListResponse GetIntuityFulfillmentRequestFailedList()
        {
            GetIntuityFulfillmentRequestsListResponse response = new GetIntuityFulfillmentRequestsListResponse();
            response.IntuityFulfillmentRequests = context.IntuityFulfillmentRequests.Include(i => i.IntuityEligibility).Include(i => i.IntuityEligibility.Organization).GroupBy(x => x.IntuityEligibilityId).Select(x => x.OrderByDescending(y => y.CreatedOn).FirstOrDefault()).ToList().Where(x => x.Submitted == false).ToList();
            return response;
        }

        public GetIntuityQuantityonHandFailedListResponse GetIntuityQuantityonHandFailedList()
        {
            GetIntuityQuantityonHandFailedListResponse response = new GetIntuityQuantityonHandFailedListResponse();
            response.IntuityQOHList = context.IntuityQOHs.Include(i => i.IntuityEligibility).Include(i => i.IntuityEligibility.Organization).GroupBy(x => x.IntuityEligibilityId).Select(x => x.OrderByDescending(y => y.CreatedOn).FirstOrDefault()).ToList().Where(x => x.Submitted == false).ToList();
            return response;
        }

        public GetIntuityFulfillmentRequestsResponse GetIntuityFulfillmentRequestsCountByPortalDate(GetIntuityFulfillmentRequestsListRequest request)
        {
            GetIntuityFulfillmentRequestsResponse response = new GetIntuityFulfillmentRequestsResponse();
            DateTime startDate = context.Portals.Where(p => p.OrganizationId == request.Organizationid).Select(p => p.StartDate).FirstOrDefault().Date;
            var intuityFulfillmentRequests = context.IntuityFulfillmentRequests.Where(x => x.IntuityEligibilityId == request.IntuityEligibilityId && x.CreatedOn >= startDate).OrderByDescending(x => x.CreatedOn).ToList();
            response.IntuityFulfillmentRequests = Utility.mapper.Map<IList<DAL.IntuityFulfillmentRequests>, IList<IntuityFulfillmentRequestsDto>>(intuityFulfillmentRequests);
            return response;
        }

        public GetIntuityEligibilityLogListResponse GetIntuityEligibilityLogRequestFailedList(GetIntuityEligibilityLogListRequest request)
        {
            GetIntuityEligibilityLogListResponse response = new GetIntuityEligibilityLogListResponse();
            response.IntuityEligibilityLogList = context.IntuityEligibilityLogs.Where(x => x.APIStatus == (byte)request.SuccessStatus || x.APIStatus == (byte)request.FailedStatus).GroupBy(x => x.UniqueId).Select(x => x.OrderByDescending(y => y.DateCreated).FirstOrDefault()).Where(y => y.APIStatus == (byte)request.FailedStatus).ToList();
            return response;
        }

        public GetIntuityUserExternalIdResponse GetIntuityUserExternalId(GetIntuityUserExternalIdRequest request)
        {
            GetIntuityUserExternalIdResponse response = new GetIntuityUserExternalIdResponse();
            response.ExternalId = context.Users.Include("IntuityUsers").Where(x => x.UniqueId == request.UniqueId && x.OrganizationId == request.OrganizationId).FirstOrDefault().IntuityUsers.ExternalUserId.ToString();
            return response;
        }

        public UpdateIntuityResponse UpdateIntuityEligibility(UpdateIntuityRequest request)
        {
            UpdateIntuityResponse response = new UpdateIntuityResponse();
            AddEditIntuityEligibilityRequest intuityEligibilityRequest = new AddEditIntuityEligibilityRequest();
            IntuityEligibilityDto intuityEligibility = new IntuityEligibilityDto();
            intuityEligibility.UniqueId = request.IntuityEligibilityLog.UniqueId;
            intuityEligibility.OrganizationId = request.IntuityEligibilityLog.OrganizationId.Value;
            if (request.UpdateProfile)
            {
                var intuityEligibilityLog = context.IntuityEligibilityLogs.Where(x => x.UniqueId == request.IntuityEligibilityLog.UniqueId && x.OrganizationId == request.IntuityEligibilityLog.OrganizationId).OrderByDescending(x => x.DateCreated).FirstOrDefault();
                if (intuityEligibilityLog != null)
                {
                    var intuityEligibilityLogUpdated = IntuityEligibilityLogDto.MapToIntuityEligibilityLog(intuityEligibilityLog);
                    intuityEligibility.FirstName = intuityEligibilityLogUpdated.FirstName = request.IntuityEligibilityLog.FirstName;
                    intuityEligibility.LastName = intuityEligibilityLogUpdated.LastName = request.IntuityEligibilityLog.LastName;
                    intuityEligibility.email = intuityEligibilityLogUpdated.email = request.IntuityEligibilityLog.email;
                    intuityEligibility.PhoneNumber = intuityEligibilityLogUpdated.PhoneNumber = request.IntuityEligibilityLog.PhoneNumber;
                    intuityEligibility.AddressLine1 = intuityEligibilityLogUpdated.AddressLine1 = request.IntuityEligibilityLog.AddressLine1;
                    intuityEligibility.AddressLine2 = intuityEligibilityLogUpdated.AddressLine2 = request.IntuityEligibilityLog.AddressLine2;
                    intuityEligibility.City = intuityEligibilityLogUpdated.City = request.IntuityEligibilityLog.City;
                    intuityEligibility.Country = intuityEligibilityLogUpdated.Country = request.IntuityEligibilityLog.Country;
                    intuityEligibility.State = intuityEligibilityLogUpdated.State = request.IntuityEligibilityLog.State;
                    intuityEligibility.Zip = intuityEligibilityLogUpdated.Zip = request.IntuityEligibilityLog.Zip;
                    intuityEligibilityLogUpdated.DateCreated = DateTime.UtcNow;
                    intuityEligibilityLogUpdated.CreatedBy = request.UserId;
                    context.IntuityEligibilityLogs.Add(intuityEligibilityLogUpdated);
                    context.SaveChanges();
                    response.Status = true;
                    intuityEligibilityRequest.UserId = request.UserId;
                    intuityEligibilityRequest.UpdateStatus = false;
                    intuityEligibilityRequest.IntuityEligibility = intuityEligibility;
                    AddEditIntuityEligibility(intuityEligibilityRequest);
                }
            }
            else
            {
                EligibilityReader reader = new EligibilityReader();
                ParticipantReader partReader = new ParticipantReader();
                var eligibility = partReader.GetEligibilityByUniqueId(request.IntuityEligibilityLog.UniqueId, request.PortalId);
                IntuityEligibilityLog intuityEligibilityLog;
                request.IntuityEligibilityLog.APIStatus = (byte)APIStatus.SuccessProfile;
                request.IntuityEligibilityLog.DateCreated = DateTime.UtcNow;
                request.IntuityEligibilityLog.CreatedBy = request.UserId;
                request.IntuityEligibilityLog.EligibilityId = eligibility.Id;
                request.IntuityEligibilityLog.EligibilityStatus = (byte)EligibilityStatus.NotEligible;
                request.IntuityEligibilityLog.EligibilityReason = (byte)EligibilityReason.NoMedHistoryLabNotOutofRange;

                response.EligibilityStatus = EligibilityStatus.NotEligible.ToString();
                response.EligibilityReason = GetDescription(EligibilityReason.NoMedHistoryLabNotOutofRange);

                intuityEligibility.EligibilityStatus = EligibilityStatus.NotEligible;
                intuityEligibility.EligibilityReason = EligibilityReason.NoMedHistoryLabNotOutofRange;

                if (eligibility.UserStatus.Key != EligibilityUserStatusDto.Terminated.Key)
                {
                    //HasDiabete
                    if ((request.IntuityEligibilityLog.HasDiabetes.HasValue && request.IntuityEligibilityLog.HasDiabetes.Value == 1) || (request.IntuityEligibilityLog.TakeDiabetesMed.HasValue && request.IntuityEligibilityLog.TakeDiabetesMed.Value == 1))
                    {
                        request.IntuityEligibilityLog.EligibilityStatus = (byte)EligibilityStatus.Eligible;
                        request.IntuityEligibilityLog.EligibilityReason = (byte)EligibilityReason.AnsweredDiabetes;

                        response.EligibilityStatus = EligibilityStatus.Eligible.ToString();
                        response.EligibilityReason = GetDescription(EligibilityReason.AnsweredDiabetes);

                        intuityEligibility.EligibilityStatus = EligibilityStatus.Eligible;
                        intuityEligibility.EligibilityReason = EligibilityReason.AnsweredDiabetes;
                    }
                    //SatisfiesA1CRange
                    else if ((request.IntuityEligibilityLog.HadA1CTest.HasValue && request.IntuityEligibilityLog.HadA1CTest.Value == 1) && request.IntuityEligibilityLog.A1CValue.HasValue && request.IntuityEligibilityLog.A1CValue.Value >= 6.5)
                    {
                        request.IntuityEligibilityLog.EligibilityStatus = (byte)EligibilityStatus.Eligible;
                        request.IntuityEligibilityLog.EligibilityReason = (byte)EligibilityReason.A1C_OutOfRange;

                        response.EligibilityStatus = EligibilityStatus.Eligible.ToString();
                        response.EligibilityReason = GetDescription(EligibilityReason.A1C_OutOfRange);

                        intuityEligibility.EligibilityStatus = EligibilityStatus.Eligible;
                        intuityEligibility.EligibilityReason = EligibilityReason.A1C_OutOfRange;
                    }
                }
                else
                {
                    request.IntuityEligibilityLog.EligibilityReason = (byte)EligibilityReason.Terminated;
                    response.EligibilityReason = GetDescription(EligibilityReason.Terminated);
                    intuityEligibility.EligibilityReason = EligibilityReason.Terminated;
                }

                intuityEligibilityLog = Utility.mapper.Map<IntuityEligibilityLogDto, IntuityEligibilityLog>(request.IntuityEligibilityLog);
                context.IntuityEligibilityLogs.Add(intuityEligibilityLog);
                context.SaveChanges();
                intuityEligibility.FirstName = request.IntuityEligibilityLog.FirstName;
                intuityEligibility.LastName = request.IntuityEligibilityLog.LastName;
                intuityEligibility.email = request.IntuityEligibilityLog.email;
                intuityEligibility.PhoneNumber = request.IntuityEligibilityLog.PhoneNumber;
                intuityEligibility.AddressLine1 = request.IntuityEligibilityLog.AddressLine1;
                intuityEligibility.AddressLine2 = request.IntuityEligibilityLog.AddressLine2;
                intuityEligibility.City = request.IntuityEligibilityLog.City;
                intuityEligibility.Country = request.IntuityEligibilityLog.Country;
                intuityEligibility.State = request.IntuityEligibilityLog.State;
                intuityEligibility.Zip = request.IntuityEligibilityLog.Zip;
                intuityEligibilityRequest.IntuityEligibility = intuityEligibility;
                intuityEligibilityRequest.UpdateStatus = true;
                intuityEligibilityRequest.UserId = request.UserId;
                response.Status = AddEditIntuityEligibility(intuityEligibilityRequest).Status;
            }
            return response;
        }

        public AddEditIntuityEligibilityResponse AddEditIntuityEligibility(AddEditIntuityEligibilityRequest request)
        {
            AddEditIntuityEligibilityResponse response = new AddEditIntuityEligibilityResponse();
            var intuityEligibility = context.IntuityEligibilities.Where(x => x.UniqueId == request.IntuityEligibility.UniqueId && x.OrganizationId == request.IntuityEligibility.OrganizationId).FirstOrDefault();
            if (intuityEligibility != null)
            {
                intuityEligibility.FirstName = request.IntuityEligibility.FirstName;
                intuityEligibility.LastName = request.IntuityEligibility.LastName;
                intuityEligibility.email = request.IntuityEligibility.email;
                intuityEligibility.PhoneNumber = request.IntuityEligibility.PhoneNumber;
                intuityEligibility.AddressLine1 = request.IntuityEligibility.AddressLine1;
                intuityEligibility.AddressLine2 = request.IntuityEligibility.AddressLine2;
                intuityEligibility.City = request.IntuityEligibility.City;
                intuityEligibility.Country = request.IntuityEligibility.Country;
                intuityEligibility.State = request.IntuityEligibility.State;
                intuityEligibility.Zip = request.IntuityEligibility.Zip;
                intuityEligibility.DateUpdated = DateTime.UtcNow;
                intuityEligibility.UpdatedBy = request.UserId;
                if (request.UpdateStatus)
                {
                    intuityEligibility.EligibilityStatus = (byte)request.IntuityEligibility.EligibilityStatus;
                    intuityEligibility.EligibilityReason = (byte)request.IntuityEligibility.EligibilityReason;
                }
                context.IntuityEligibilities.Attach(intuityEligibility);
                context.Entry(intuityEligibility).State = EntityState.Modified;
            }
            else
            {
                intuityEligibility = IntuityEligibilityDto.MapToIntuityEligibility(request.IntuityEligibility);
                intuityEligibility.DateCreated = DateTime.UtcNow;
                context.IntuityEligibilities.Add(intuityEligibility);
            }
            context.SaveChanges();
            response.Status = true;
            return response;
        }

        public UpdateEligiblityStatusResponse UpdateEligiblityStatus(UpdateEligiblityStatusRequest request)
        {
            UpdateEligiblityStatusResponse response = new UpdateEligiblityStatusResponse();
            var intuityEligibilityLog = context.IntuityEligibilityLogs.Where(x => x.UniqueId == request.UniqueId && x.OrganizationId == request.OrganizationId).OrderByDescending(x => x.DateCreated).FirstOrDefault();
            if (intuityEligibilityLog != null)
            {
                var intuityEligibilityLogUpdated = IntuityEligibilityLogDto.MapToIntuityEligibilityLog(intuityEligibilityLog);
                intuityEligibilityLogUpdated.EligibilityReason = (byte)request.EligiblityReason;
                intuityEligibilityLogUpdated.EligibilityStatus = (byte)request.EligiblityStatus;
                intuityEligibilityLogUpdated.DateCreated = DateTime.UtcNow;
                intuityEligibilityLogUpdated.CreatedBy = request.CreatedBy;
                context.IntuityEligibilityLogs.Add(intuityEligibilityLogUpdated);
                context.SaveChanges();
                response.Status = true;
            }

            var intuityEligibility = context.IntuityEligibilities.Where(x => x.UniqueId == request.UniqueId && x.OrganizationId == request.OrganizationId).FirstOrDefault();
            if (intuityEligibility != null)
            {
                intuityEligibility.EligibilityStatus = (byte)request.EligiblityStatus;
                intuityEligibility.EligibilityReason = (byte)request.EligiblityReason;
                intuityEligibility.OverrideStatus = true;
                context.IntuityEligibilities.Attach(intuityEligibility);
                context.Entry(intuityEligibility).State = EntityState.Modified;
                context.SaveChanges();
                response.Status = true;
            }
            return response;
        }

        public UpdateIntuityFulfillmentResponse UpdateIntuityFulfillmentRequest(UpdateIntuityFulfillmentRequest request)
        {
            UpdateIntuityFulfillmentResponse response = new UpdateIntuityFulfillmentResponse();
            IntuityFulfillmentRequests intuityFulfillmentRequest = new IntuityFulfillmentRequests();
            intuityFulfillmentRequest.IntuityEligibilityId = request.IntuityEligibilityId;
            intuityFulfillmentRequest.ImmediateShipment = request.ImmediateShipment;
            intuityFulfillmentRequest.ReplenishmentQuantity = request.Quantity;
            intuityFulfillmentRequest.Reason = request.Reason;
            intuityFulfillmentRequest.Submitted = request.Submitted;
            intuityFulfillmentRequest.CreatedOn = DateTime.UtcNow;
            intuityFulfillmentRequest.CreatedBy = request.UserId;
            intuityFulfillmentRequest.NumberOfShipments = request.numberofshipments;
            intuityFulfillmentRequest.SendMeter = request.SendMeter;
            context.IntuityFulfillmentRequests.Add(intuityFulfillmentRequest);
            context.SaveChanges();
            response.IntuityFulfillmentRequests = GetIntuityFulfillmentRequestsDetails(intuityFulfillmentRequest).IntuityFulfillmentRequests;
            response.Status = true;
            return response;
        }

        public UpdateIntuityQuantityOnHandResponse UpdateIntuityQuantityonHand(UpdateIntuityQuantityOnHandRequest request)
        {
            UpdateIntuityQuantityOnHandResponse response = new UpdateIntuityQuantityOnHandResponse();
            IntuityQOH qoh = new IntuityQOH();
            qoh.IntuityEligibilityId = request.IntuityEligibilityId;
            qoh.Submitted = request.Submitted;
            qoh.CreatedOn = DateTime.UtcNow;
            qoh.QuantityOnHand = request.Quantity;
            qoh.CreatedBy = request.UserId;
            context.IntuityQOHs.Add(qoh);
            context.SaveChanges();
            response.Status = true;
            return response;
        }

        public UpdateIntuityOptingOutResponse UpdateIntuityOptingOutRequest(UpdateIntuityOptingOutRequest request)
        {
            UpdateIntuityOptingOutResponse response = new UpdateIntuityOptingOutResponse();
            if (request.OptingOut)
                response.OptingOut = request.OptedOutDate;

            IntuityEligibilityLog intuityEligibilityLog = context.IntuityEligibilityLogs.Where(x => x.UniqueId == request.IntuityEligibilityLog.UniqueId && x.OrganizationId == request.IntuityEligibilityLog.OrganizationId).OrderByDescending(x => x.DateCreated).FirstOrDefault();
            if (intuityEligibilityLog != null)
            {
                IntuityEligibilityLog intuityEligibilityLogUpdated = IntuityEligibilityLogDto.MapToIntuityEligibilityLog(intuityEligibilityLog);
                intuityEligibilityLogUpdated.APIStatus = (byte)APIStatus.FailedOptingOut;
                if (request.OptingOut)
                    intuityEligibilityLogUpdated.OptingOut = request.OptedOutDate;
                else
                    intuityEligibilityLogUpdated.OptingOut = null;
                intuityEligibilityLogUpdated.DateCreated = DateTime.UtcNow;
                intuityEligibilityLogUpdated.CreatedBy = request.UserId;
                context.IntuityEligibilityLogs.Add(intuityEligibilityLogUpdated);
                context.SaveChanges();
                response.Status = true;
            }
            else
            {
                IntuityEligibilityLog intuityEligibilityLogAdd = Utility.mapper.Map<IntuityEligibilityLogDto, IntuityEligibilityLog>(request.IntuityEligibilityLog);
                intuityEligibilityLogAdd.APIStatus = (byte)APIStatus.FailedOptingOut;
                intuityEligibilityLogAdd.OptingOut = request.OptedOutDate;
                intuityEligibilityLogAdd.DateCreated = DateTime.UtcNow;
                intuityEligibilityLogAdd.CreatedBy = request.UserId;
                context.IntuityEligibilityLogs.Add(intuityEligibilityLogAdd);
                context.SaveChanges();
                response.Status = true;

            }
            IntuityEligibility intuityEligibility = context.IntuityEligibilities.Where(x => x.UniqueId == request.IntuityEligibilityLog.UniqueId && x.OrganizationId == request.IntuityEligibilityLog.OrganizationId).FirstOrDefault();
            if (intuityEligibility != null)
            {
                if (request.OptingOut)
                    intuityEligibility.OptingOut = request.OptedOutDate;
                else
                    intuityEligibility.OptingOut = null;
                intuityEligibility.DateUpdated = DateTime.UtcNow;
                intuityEligibility.UpdatedBy = request.UserId;
                context.IntuityEligibilities.Attach(intuityEligibility);
                context.Entry(intuityEligibility).State = EntityState.Modified;
                context.SaveChanges();
                response.Status = true;
            }
            return response;
        }

        public UpdatePatternPairingDateResponse UpdatePatternPairingDateRequest(UpdatePatternPairingDateRequest request)
        {
            UpdatePatternPairingDateResponse response = new UpdatePatternPairingDateResponse();
            IntuityEligibilityLog intuityEligibilityLog = context.IntuityEligibilityLogs.Where(x => x.UniqueId == request.IntuityEligibilityLog.UniqueId && x.OrganizationId == request.IntuityEligibilityLog.OrganizationId).OrderByDescending(x => x.DateCreated).FirstOrDefault();
            if (intuityEligibilityLog != null)
            {
                IntuityEligibilityLog intuityEligibilityLogUpdated = IntuityEligibilityLogDto.MapToIntuityEligibilityLog(intuityEligibilityLog);
                intuityEligibilityLogUpdated.PairedDate = request.PairingDate;
                intuityEligibilityLogUpdated.Devices = request.Devices;
                intuityEligibilityLogUpdated.DateCreated = DateTime.UtcNow;
                intuityEligibilityLogUpdated.CreatedBy = request.systemAdminId;
                context.IntuityEligibilityLogs.Add(intuityEligibilityLogUpdated);
                context.SaveChanges();
                response.Status = true;
            }
            return response;
        }

        public bool UpdatePatternCreationRequest(UpdatePatternCreationRequest request)
        {
            IntuityEligibilityLog intuityEligibilityLog = context.IntuityEligibilityLogs.Where(x => x.UniqueId == request.IntuityEligibilityLog.UniqueId && x.OrganizationId == request.IntuityEligibilityLog.OrganizationId).OrderByDescending(x => x.DateCreated).FirstOrDefault();
            if (intuityEligibilityLog != null)
            {
                IntuityEligibilityLog intuityEligibilityLogUpdated = IntuityEligibilityLogDto.MapToIntuityEligibilityLog(intuityEligibilityLog);
                intuityEligibilityLogUpdated.PatternsRegDate = request.PatternCreationDate;
                intuityEligibilityLogUpdated.DateCreated = DateTime.UtcNow;
                intuityEligibilityLogUpdated.CreatedBy = request.systemAdminId;
                context.IntuityEligibilityLogs.Add(intuityEligibilityLogUpdated);
                context.SaveChanges();
            }
            return true;
        }

        public bool BulkUpdateIntuityFulfillmentRequestList(BulkUpdateIntuityFulfillmentRequest request)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                //context.Configuration.AutoDetectChangesEnabled = false;
                foreach (IntuityFulfillmentRequests dto in request.IntuityFulfillmentRequestsList)
                {
                    context.IntuityFulfillmentRequests.Attach(dto);
                    context.Entry(dto).State = EntityState.Modified;
                }
                context.SaveChanges();
                scope.Complete();
            }
            return true;
        }

        public bool BulkUpdateIntuityQuantityOnHandList(BulkUpdateIntuityQuantityOnHandListRequest request)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                //context.Configuration.AutoDetectChangesEnabled = false;
                foreach (IntuityQOH dto in request.IntuityQOHRequestsList)
                {
                    context.IntuityQOHs.Attach(dto);
                    context.Entry(dto).State = EntityState.Modified;
                }
                context.SaveChanges();
                scope.Complete();
            }
            return true;
        }

        public bool BulkUpdateIntuityEligibilityLogRequestList(BulkUpdateIntuityEligibilityLogRequest request)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                //context.Configuration.AutoDetectChangesEnabled = false;
                foreach (IntuityEligibilityLog dto in request.IntuityEligibilityLogList)
                {
                    context.IntuityEligibilityLogs.Attach(dto);
                    context.Entry(dto).State = EntityState.Modified;
                }
                context.SaveChanges();
                scope.Complete();
            }
            return true;
        }

        public GetIntuityEligibilityResponse GetEligibilityByNameandDOB(int portalId, string firstName, string lastName, DateTime dob)
        {
            GetIntuityEligibilityResponse response = new GetIntuityEligibilityResponse();
            var eligibility = context.Eligibilities.Where(x => x.PortalId == portalId && x.FirstName == firstName && x.LastName == lastName && x.DOB == dob).ToList();
            if (eligibility != null)
            {
                response.Count = eligibility.Count;
                if (eligibility.Count == 1)
                {
                    response.Status = true;
                    response.Eligibility = Utility.mapper.Map<DAL.Eligibility, EligibilityDto>(eligibility[0]);
                }
            }
            return response;
        }

        public static string GetDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description
                ?? value.ToString();
        }

        public int ProcessFutureAppointmentEvent(int systemAdminId)
        {
            ExternalReader externalReader = new ExternalReader();
            var appointments = context.Appointments.Include("User").Include("User.Organization").Include("User.Organization.Portals")
                .Where(x => x.Date >= DateTime.UtcNow && x.Active && x.User.IsActive && x.User.Organization.Portals.Any(y => y.Active == true)
                && x.User.Organization.IntegrationWith.HasValue && x.User.Organization.IntegrationWith.Value == (byte)IntegrationPartner.Intuity).ToList();
            foreach (var appointment in appointments)
            {
                if (externalReader.GetIntuityUsersByUserId(appointment.User.Id) != null)
                {
                    AddIntuityEventRequest intuityEventRequest = new AddIntuityEventRequest();
                    intuityEventRequest.intuityEvent = new IntuityEventDto();
                    intuityEventRequest.intuityEvent.UserId = appointment.User.Id;
                    intuityEventRequest.intuityEvent.EventType = (int)IntuityEventTypes.Future_Appointment;
                    intuityEventRequest.organizationCode = appointment.User.Organization.Code;
                    intuityEventRequest.intuityEvent.UniqueId = appointment.User.UniqueId;
                    intuityEventRequest.intuityEvent.EventDate = DateTime.UtcNow;
                    intuityEventRequest.intuityEvent.CreatedBy = systemAdminId;
                    AddIntuityEvent(intuityEventRequest);
                }
            }
            return appointments.Count;
        }

        public List<IntuityActivityCsvModel> GetIntuityActivityList(int systemAdminId)
        {
            List<IntuityActivityCsvModel> activityList = new List<IntuityActivityCsvModel>();
            //Process records
            var intuityEvents = context.IntuityEvents.Include("IntuityEventType").Where(x => x.Processed == false).ToList();
            foreach (var intuityEvent in intuityEvents)
            {
                if (!IsOptedOut(intuityEvent.UniqueId))
                {
                    IntuityActivityCsvModel intuityActivity = new IntuityActivityCsvModel();
                    intuityActivity.EventType = intuityEvent.IntuityEventType.Type;
                    intuityActivity.UniqueID = intuityEvent.UniqueId;
                    intuityActivity.DateTimeStamp = intuityEvent.EventDate;
                    activityList.Add(intuityActivity);

                    intuityEvent.Processed = true;
                    intuityEvent.UpdatedBy = systemAdminId;
                    intuityEvent.UpdatedOn = DateTime.UtcNow;
                    context.IntuityEvents.Attach(intuityEvent);
                    context.Entry(intuityEvent).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            return activityList;
        }

        public void AddIntuityEvent(AddIntuityEventRequest request)
        {

            string externalUserId = "";
            if (request.organizationCode == "DTCOrgCode")
            {
                ExternalReader externalReader = new ExternalReader();
                var intuityUser = externalReader.GetIntuityUsersByUserId(request.intuityEvent.UserId);
                externalUserId = "-" + intuityUser.ExternalUserId;
            }
            request.intuityEvent.UniqueId = request.organizationCode + "-" + request.intuityEvent.UniqueId + externalUserId;
            if (!IsOptedOut(request.intuityEvent.UniqueId))
            {
                IntuityEvent intuityEvent = new IntuityEvent();
                intuityEvent.UserId = request.intuityEvent.UserId;
                intuityEvent.UniqueId = request.intuityEvent.UniqueId;
                intuityEvent.EventType = request.intuityEvent.EventType;
                intuityEvent.EventDate = request.intuityEvent.EventDate;
                intuityEvent.CreatedOn = DateTime.UtcNow;
                intuityEvent.CreatedBy = request.intuityEvent.CreatedBy;
                context.IntuityEvents.Add(intuityEvent);
                context.SaveChanges();
            }
        }

        public bool IsOptedOut(string orgCodeAndUniqueId)
        {
            PortalReader portalReader = new PortalReader();
            string uniqueId = orgCodeAndUniqueId.Split('-')[1];
            int organizationId = portalReader.GetOrganizationByCode(orgCodeAndUniqueId.Split('-').First()).Id.Value;
            var response = portalReader.CurrentPortalIdForOrganization(new ListPortalsRequest { organizationId = organizationId });
            if (response.PortalId.HasValue)
            {
                var eligibility = context.Eligibilities.Where(x => x.UniqueId == uniqueId && x.PortalId == response.PortalId).FirstOrDefault();
                if (eligibility == null || eligibility.UserStatus == EligibilityUserStatusDto.Terminated.Key)
                    return true;
                if (orgCodeAndUniqueId.Split('-').First() == "DTCOrgCode")
                    return false;
                var intuityEligibility = context.IntuityEligibilities.Where(x => x.UniqueId == uniqueId && x.OrganizationId == organizationId && !string.IsNullOrEmpty(x.SerialNumber) && !x.OptingOut.HasValue).FirstOrDefault();
                if (intuityEligibility != null)
                    return false;
            }
            return true;
        }

        public GetAccessTokenResponse GetIntuityRefreshToken()
        {
            GetAccessTokenResponse response = new GetAccessTokenResponse();
            DateTime date = DateTime.UtcNow.AddDays(+3).Date;
            var count = context.AccessTokens.Where(x => x.OrgName.Equals(AccessToken.Intuity) && x.Name.Equals(AccessToken.Token) && x.ExpiryDate.Value <= date).Count();
            if (count > 0)
            {
                var token = context.AccessTokens.Where(x => x.OrgName == AccessToken.Intuity && x.Name == AccessToken.RefreshToken).FirstOrDefault();
                response.accessToken = Utility.mapper.Map<AccessTokens, AccessTokensDto>(token);
            }
            return response;
        }

        public bool UpdateIntuityAccessToken(IntuityRefreshTokenResponse response)
        {
            bool status = false;
            var refreshToken = context.AccessTokens.Where(x => x.OrgName.Equals(AccessToken.Intuity) && x.Name.Equals(AccessToken.RefreshToken)).FirstOrDefault();
            if (refreshToken != null)
            {
                refreshToken.ExpiryDate = response.refreshToken.refresh_token_expiry_date;
                refreshToken.Token = response.refreshToken.refresh_token;
                context.AccessTokens.Attach(refreshToken);
                context.Entry(refreshToken).State = EntityState.Modified;
                context.SaveChanges();

                var accessToken = context.AccessTokens.Where(x => x.OrgName.Equals(AccessToken.Intuity) && x.Name.Equals(AccessToken.Token)).FirstOrDefault();
                if (accessToken != null)
                {
                    accessToken.ExpiryDate = response.refreshToken.access_token_expiry_date;
                    accessToken.Token = response.refreshToken.access_token;
                    context.AccessTokens.Attach(accessToken);
                    context.Entry(accessToken).State = EntityState.Modified;
                    context.SaveChanges();
                    status = true;
                }
            }
            return status;
        }

        public List<IntuityEligibility> GetIntuityUsersList()
        {
            return context.IntuityEligibilities.Include("Organization").Where(x => x.EligibilityStatus == (byte)EligibilityStatus.Eligible || x.EligibilityStatus == (byte)EligibilityStatus.NotEligible).ToList();
        }

        public bool AddEditIntuityEPData(int intuityEligibilityId, IntuityEligibilityLogAPIResponse.Patient patientDetails)
        {
            IntuityEPData data = new IntuityEPData();
            var epData = context.IntuityEPData.Where(x => x.IntuityEligibilityId == intuityEligibilityId).FirstOrDefault();
            if (epData == null)
            {
                data.IntuityEligibilityId = intuityEligibilityId;
                data.PairMonitorCount = patientDetails.pair_monitor_reminders_count;
                data.PatternsRegCount = patientDetails.patterns_registration_reminders_count;
                data.SyncTestResultCount = patientDetails.sync_test_result_reminders_count;
                data.CreatedOn = DateTime.UtcNow;
                context.IntuityEPData.Add(data);
                context.SaveChanges();
                return true;
            }
            else if (epData.PairMonitorCount != patientDetails.pair_monitor_reminders_count || epData.PatternsRegCount != patientDetails.patterns_registration_reminders_count || epData.SyncTestResultCount != patientDetails.sync_test_result_reminders_count)
            {
                epData.PairMonitorCount = patientDetails.pair_monitor_reminders_count;
                epData.PatternsRegCount = patientDetails.patterns_registration_reminders_count;
                epData.SyncTestResultCount = patientDetails.sync_test_result_reminders_count;
                epData.UpdatedOn = DateTime.UtcNow;
                context.IntuityEPData.Attach(epData);
                context.Entry(epData).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool CheckGlucoseActivity()
        {
            var lastDate = DateTime.UtcNow.AddDays(-1).Date;
            if (context.EXT_Glucose.Where(x => x.Source == (byte)GlucoseSource.Intuity && x.DateTime.Value.Date == lastDate).Count() == 0)
            {
                return true;
            }
            return false;
        }
    }
}
