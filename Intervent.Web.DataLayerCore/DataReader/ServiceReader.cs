using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class ServiceReader
    {
        InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public ListOutreachResponse GetOutreachList(ListOutreachRequest request)
        {
            ListOutreachResponse response = new ListOutreachResponse();
            StoredProcedures sp = new StoredProcedures();
            var OutreachDAL = sp.GetDataforOutreach();
            if (OutreachDAL != null && OutreachDAL.Count > 0)
            {
                List<OutreachLogDto> OutreachList = new List<OutreachLogDto>();
                for (int i = 0; i < OutreachDAL.Count; i++)
                {
                    OutreachLogDto Outreach = new OutreachLogDto();
                    Outreach.City = OutreachDAL[i].City;
                    Outreach.Company = OutreachDAL[i].Company;
                    Outreach.FirstName = OutreachDAL[i].FirstName;
                    Outreach.Language = OutreachDAL[i].Language;
                    Outreach.LastName = OutreachDAL[i].LastName;
                    Outreach.OrgId = OutreachDAL[i].OrgId;
                    Outreach.PhoneNumber1 = OutreachDAL[i].PhoneNumber1;
                    Outreach.PortalId = OutreachDAL[i].PortalId;
                    Outreach.State = OutreachDAL[i].State;
                    Outreach.Street = OutreachDAL[i].Street;
                    Outreach.UserId = OutreachDAL[i].UserId;
                    Outreach.Zip = OutreachDAL[i].Zip;
                    OutreachList.Add(Outreach);
                }
                response.OutreachList = OutreachList;
            }
            return response;
        }

        public void LogOutreach(LogOutreachRequest request)
        {
            //save outreach log
            LogOutreachResponse response = new LogOutreachResponse();
            var OutreachLog = Utility.mapper.Map<OutreachLogDto, DAL.OutreachLog>(request.OutreachLog);
            context.OutreachLogs.Add(OutreachLog);
            context.SaveChanges();
        }

        public ListOutreachResponse GetTrackingList(ListOutreachRequest request)
        {
            ListOutreachResponse response = new ListOutreachResponse();
            StoredProcedures sp = new StoredProcedures();
            var OutreachDAL = sp.GetDataforTracking();
            if (OutreachDAL != null && OutreachDAL.Count > 0)
            {
                List<OutreachLogDto> OutreachList = new List<OutreachLogDto>();
                for (int i = 0; i < OutreachDAL.Count; i++)
                {
                    OutreachLogDto Outreach = new OutreachLogDto();
                    Outreach.City = OutreachDAL[i].City;
                    Outreach.Company = OutreachDAL[i].Company;
                    Outreach.FirstName = OutreachDAL[i].FirstName;
                    Outreach.Language = OutreachDAL[i].Language;
                    Outreach.LastName = OutreachDAL[i].LastName;
                    Outreach.OrgId = OutreachDAL[i].OrgId;
                    Outreach.PhoneNumber1 = OutreachDAL[i].PhoneNumber1;
                    Outreach.PortalId = OutreachDAL[i].PortalId;
                    Outreach.State = OutreachDAL[i].State;
                    Outreach.Street = OutreachDAL[i].Street;
                    Outreach.UserId = OutreachDAL[i].UserId;
                    Outreach.Zip = OutreachDAL[i].Zip;
                    OutreachList.Add(Outreach);
                }
                response.OutreachList = OutreachList;
            }
            return response;
        }

        public ListCallReminderResponse GetCallReminderList()
        {
            ListCallReminderResponse response = new ListCallReminderResponse();
            DateTime toDate = DateTime.UtcNow.AddDays(2);
            var appointments = context.Appointments.Include("User").Include("User.Organization").Include("User.Organization.Portals").Include("User.TimeZone")
                .Where(x => x.Active && x.Date > DateTime.UtcNow && x.Date < toDate && x.Type != 4 && x.Type != 5 && x.User.IsActive == true && x.User.Organization.Portals.Where(y => y.Active == true).FirstOrDefault().AppointmentCalls == true).ToList();
            if (appointments != null && appointments.Count > 0)
            {
                List<CallReminderLogDto> CallReminderList = new List<CallReminderLogDto>();
                for (int i = 0; i < appointments.Count; i++)
                {
                    CallReminderLogDto CallReminder = new CallReminderLogDto();
                    CallReminder.UserId = appointments[i].UserId;
                    CallReminder.AppointmentId = appointments[i].Id;
                    CallReminder.FirstName = appointments[i].User.FirstName;
                    CallReminder.LastName = appointments[i].User.LastName;
                    CallReminder.Language = appointments[i].User.LanguagePreference;
                    CallReminder.ClientPhone = appointments[i].User.Organization.ContactNumber;
                    if (appointments[i].User.ContactMode.HasValue)
                    {
                        if (appointments[i].User.ContactMode == 1)
                            CallReminder.PhoneNumber = appointments[i].User.HomeNumber;
                        else if (appointments[i].User.ContactMode == 2)
                            CallReminder.PhoneNumber = appointments[i].User.WorkNumber;
                        else if (appointments[i].User.ContactMode == 3)
                            CallReminder.PhoneNumber = appointments[i].User.CellNumber;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(appointments[i].User.CellNumber))
                        {
                            CallReminder.ContactMode = "Mobile";
                            CallReminder.PhoneNumber = appointments[i].User.CellNumber;
                        }
                        else if (!string.IsNullOrEmpty(appointments[i].User.HomeNumber))
                        {
                            CallReminder.ContactMode = "Home";
                            CallReminder.PhoneNumber = appointments[i].User.HomeNumber;
                        }
                        else if (!string.IsNullOrEmpty(appointments[i].User.WorkNumber))
                            CallReminder.PhoneNumber = appointments[i].User.WorkNumber;
                    }
                    if (!string.IsNullOrEmpty(CallReminder.PhoneNumber))
                    {
                        //Date in user timezone
                        TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(appointments[i].User.TimeZone.TimeZoneId);
                        CallReminder.TimezoneId = appointments[i].User.TimeZone.TimeZoneId;
                        CallReminder.Date = TimeZoneInfo.ConvertTimeFromUtc(appointments[i].Date, custTZone);
                        CallReminder.Language = appointments[i].User.LanguagePreference;
                        CallReminderList.Add(CallReminder);
                    }
                }
                response.CallReminderList = CallReminderList;
            }
            return response;
        }

        public ListCallReminderResponse GetLegacyCallReminderList()
        {
            StoredProcedures sp = new StoredProcedures();
            ListCallReminderResponse response = new ListCallReminderResponse();
            var reminderList = sp.GetCallListFromLegacySystem();
            if (reminderList != null && reminderList.Count > 0)
            {
                List<CallReminderLogDto> CallReminderList = new List<CallReminderLogDto>();
                for (int i = 0; i < reminderList.Count; i++)
                {
                    if (CallReminderList.Where(x => x.PhoneNumber == reminderList[i].PhoneNumber).Count() > 0)
                        continue;
                    CallReminderLogDto CallReminder = new CallReminderLogDto();
                    CallReminder.AppointmentId = reminderList[i].AppRef;
                    CallReminder.FirstName = reminderList[i].FirstName;
                    CallReminder.Language = reminderList[i].CurrLangCode;
                    CallReminder.ClientPhone = reminderList[i].ContactNumber;
                    CallReminder.PhoneNumber = reminderList[i].PhoneNumber;
                    if (!string.IsNullOrEmpty(CallReminder.PhoneNumber))
                    {
                        //Date in user timezone
                        TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(reminderList[i].TimeZoneID);
                        CallReminder.TimezoneId = reminderList[i].TimeZoneID;
                        CallReminder.Date = TimeZoneInfo.ConvertTimeFromUtc(reminderList[i].AppDate, custTZone);
                        CallReminder.Language = reminderList[i].CurrLangCode;
                        CallReminderList.Add(CallReminder);
                    }
                }
                response.CallReminderList = CallReminderList;
            }
            return response;
        }

        public List<LegacyAppointmentRemainderResultDto> GetTextMessageCallListFromLegacySystem()
        {
            StoredProcedures sp = new StoredProcedures();
            var response = new List<LegacyAppointmentRemainderResultDto>();
            var reminderList = sp.GetTextMessageCallListFromLegacySystem();
            foreach (var reminder in reminderList)
            {
                var model = new LegacyAppointmentRemainderResultDto();
                model.AppRef = reminder.AppRef;
                model.FirstName = reminder.FirstName;
                TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(reminder.TimeZoneID);
                model.ApptDate = TimeZoneInfo.ConvertTimeFromUtc(reminder.AppDate, custTZone);
                model.PhoneNumber = reminder.PhoneNumber;
                model.CurrLangCode = reminder.CurrLangCode;
                response.Add(model);
            }
            return response;
        }

        public void UpdateMessageSIDLegacySystem(Dictionary<int, string> messageSIDs)
        {
            StoredProcedures sp = new StoredProcedures();
            foreach (var message in messageSIDs)
            {
                sp.GetTextMessageCallListFromLegacySystem(message.Key, message.Value, "update");
            }
        }

        public class SMSReminder
        {
            public string PhoneNo { get; set; }

            public DateTime AptTime { get; set; }

            public int AptId { get; set; }

            public string ClientPhone { get; set; }

            public string FirstName { get; set; }

            public string Language { get; set; }

            public int UserId { get; set; }

            public int PortalId { get; set; }

            public string MeetingLink { get; set; }
        }

        public List<SMSReminder> GetIntuityMessageCallList()
        {
            List<SMSReminder> reminderList = new List<SMSReminder>();
            //1:15
            DateTime toDate = DateTime.UtcNow.AddDays(1).AddHours(1);
            DateTime toDateAddHour = DateTime.UtcNow.AddHours(1);
            var appointments = context.Appointments.Include("User").Include("User.Organization").Include("User.Organization.Portals").Include("User.TimeZone")
                .Where(x => x.Active && x.Date > toDateAddHour && x.Date < toDate && x.Type != 4 && x.Type != 5 && x.User.IsActive == true
                && x.User.Organization.Portals.Where(y => y.Active == true).FirstOrDefault().AppointmentCalls == true && x.User.Organization.IntegrationWith.HasValue && x.User.Organization.IntegrationWith.Value == (int)IntegrationPartner.Intuity).ToList();
            if (appointments != null && appointments.Count > 0)
            {
                List<CallReminderLogDto> CallReminderList = new List<CallReminderLogDto>();
                for (int i = 0; i < appointments.Count; i++)
                {
                    SMSReminder reminder = new SMSReminder();
                    reminder.PhoneNo = appointments[i].User.CellNumber;
                    //Date in user timezone
                    TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(appointments[i].User.TimeZone.TimeZoneId);
                    reminder.AptTime = TimeZoneInfo.ConvertTimeFromUtc(appointments[i].Date, custTZone);
                    reminder.AptId = appointments[i].Id;
                    reminder.FirstName = appointments[i].User.FirstName;
                    reminder.ClientPhone = appointments[i].User.Organization.ContactNumber;
                    reminder.Language = appointments[i].User.LanguagePreference;

                    reminder.UserId = appointments[i].UserId;
                    reminderList.Add(reminder);
                }
            }
            return reminderList;
        }


        public List<SMSReminder> GetTextMessageCallList()
        {
            List<SMSReminder> reminderList = new List<SMSReminder>();
            //1:15
            DateTime toDate = DateTime.UtcNow.AddHours(1).AddMinutes(15);
            string zoomMeetingLink = "https://zoom.us/j/";
            var appointments = context.Appointments.Include("User").Include("User.Organization").Include("User.Organization.Portals").Include("User.TimeZone")
                .Include("User1").Include("User1.AdminProperty")
                .Where(x => x.Active && x.MessageSID == null && x.Date > DateTime.UtcNow && x.Date < toDate && x.Type != 4 && x.Type != 5 && x.User.IsActive == true
                && x.User.Organization.Portals.Where(y => y.Active == true).FirstOrDefault().AppointmentCalls == true && x.User.Text == 1 && !string.IsNullOrEmpty(x.User.CellNumber)).ToList();
            if (appointments != null && appointments.Count > 0)
            {
                List<CallReminderLogDto> CallReminderList = new List<CallReminderLogDto>();
                for (int i = 0; i < appointments.Count; i++)
                {
                    SMSReminder reminder = new SMSReminder();
                    reminder.PhoneNo = appointments[i].User.CellNumber;
                    //Date in user timezone
                    TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(appointments[i].User.TimeZone.TimeZoneId);
                    reminder.AptTime = TimeZoneInfo.ConvertTimeFromUtc(appointments[i].Date, custTZone);
                    reminder.AptId = appointments[i].Id;
                    reminder.FirstName = appointments[i].User.FirstName;
                    reminder.ClientPhone = appointments[i].User.Organization.ContactNumber;
                    reminder.Language = appointments[i].User.LanguagePreference;
                    reminder.PortalId = appointments[i].User.Organization.Portals.Where(x => x.Active == true).FirstOrDefault().Id;
                    reminder.UserId = appointments[i].UserId;
                    reminder.MeetingLink = appointments[i].VideoRequired ? zoomMeetingLink + appointments[i].User1.AdminProperty.MeetingId : "";
                    reminderList.Add(reminder);
                }
            }
            return reminderList;
        }

        public User GetTextMessageCallUser(int userId)
        {
            return context.Users.Include("Organization.Portals").Where(x => x.Id == userId && x.Text == 1 && x.IsActive && !string.IsNullOrEmpty(x.CellNumber) && x.Organization.Portals.Any(y => y.Active)).FirstOrDefault();
        }

        public ListOutreachResponse GetCanriskOutreachList()
        {
            var portalId = context.Portals.Where(x => x.OrganizationId == 71 && x.Active == true).FirstOrDefault().Id;
            ListOutreachResponse response = new ListOutreachResponse();
            var canriskList = context.CanriskQuestionnaires.Include("Eligibility").Where(x => x.SentforOutreach == null && x.CompletedBy == null && x.CompletedOn != null &&
            x.isEligible == true && DateTime.Now > x.CompletedOn.Value.AddMinutes(10)).ToList();
            List<CanriskQuestionnaire> removableCanriskItems = new List<CanriskQuestionnaire>();
            foreach (var canrisk in canriskList)
            {
                bool IsHRAStarted = context.Users.Include("HRAs").Where(x => x.UniqueId == canrisk.Eligibility.UniqueId && x.HRAs.Any()).Any();
                if (IsHRAStarted)
                {
                    removableCanriskItems.Add(canrisk);
                }
            }
            foreach (var removableCanrisk in removableCanriskItems)
            {
                canriskList.Remove(removableCanrisk);
            }
            if (canriskList != null && canriskList.Count() > 0)
            {
                ParticipantReader partReader = new ParticipantReader();
                List<OutreachLogDto> OutreachList = new List<OutreachLogDto>();
                for (int i = 0; i < canriskList.Count(); i++)
                {
                    int EligibilityId = canriskList[i].Eligibility.Id;
                    OutreachLogDto Outreach = new OutreachLogDto();
                    Outreach.FirstName = canriskList[i].Eligibility.FirstName;
                    Outreach.LastName = canriskList[i].Eligibility.LastName;
                    Outreach.PhoneNumber1 = canriskList[i].Eligibility.HomeNumber;
                    GetUserEligibilitySettingRequest req = new GetUserEligibilitySettingRequest() { UniqueID = canriskList[i].Eligibility.UniqueId, OrgID = 71 };
                    var userSettingResponse = partReader.GetUserEligibilitySetting(req);
                    if (userSettingResponse != null && userSettingResponse.UserEligibilitySetting != null)
                        Outreach.Language = !String.IsNullOrEmpty(userSettingResponse.UserEligibilitySetting.Language) ? userSettingResponse.UserEligibilitySetting.Language : "en-us";
                    OutreachList.Add(Outreach);
                    var canriskRecord = context.CanriskQuestionnaires.Where(x => x.EligibilityId == EligibilityId).FirstOrDefault();
                    canriskRecord.SentforOutreach = true;
                    context.SaveChanges();
                }
                response.OutreachList = OutreachList;
            }
            return response;
        }

        public List<UserDto> GetMotivationMessageUserList(int OrgId)
        {
            List<UserDto> userlist = new List<UserDto>();

            var msgUser = context.Users.Include("Organization.Portals").Where(y => y.OrganizationId == OrgId && y.Text == 1 && y.IsActive && !string.IsNullOrEmpty(y.CellNumber) && y.Organization.Portals.Any(z => z.HasHRA == 1 && z.Active)).ToList();

            userlist = Utility.mapper.Map<List<DAL.User>, List<UserDto>>(msgUser);
            return userlist;
        }
    }
}
