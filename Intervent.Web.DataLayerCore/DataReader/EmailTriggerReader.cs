using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class EmailTriggerReader : BaseDataReader
    {
        NotificationReader _notificationReader = null;

        public EmailTriggerReader()
        {
            _notificationReader = new NotificationReader();
        }
        public ListUsersEmailTriggerResponse ListUsers(ListUsersEmailTriggerRequest request)
        {
            ListUsersEmailTriggerResponse response = new ListUsersEmailTriggerResponse();
            //var orgs = _portalReader.ListOrganizations(new ListOrganizationsRequest()).Organizations.Where(x => x.Active);
            //var currentPortalIds = new List<int>();
            //foreach(OrganizationDto org in orgs)
            //{
            //    currentPortalIds.Add(_portalReader.CurrentPortalIdForOrganization(new ListPortalsRequest() {  organizationId = org.Id}));
            //}
            //filter by current portal ids
            var eventPortalIds = _notificationReader.ListNotificationEventTypePortal(new GetNotificationEventTypePortalRequest() { NotificationEventTypeId = request.NotificationEventTypeId }).Portals.Where(x => x.Active).Select(x => x.Id);

            IQueryable<DAL.User> users = null;
            if (request.TriggerCondition == EmailTriggerCondition.IncompletProfile)
            {
                users = GetInCompleteProfileQuery(eventPortalIds);
            }
            else if (request.TriggerCondition == EmailTriggerCondition.IncompleteHRA)
            {
                users = GetHRAQuery(eventPortalIds);
            }
            else if (request.TriggerCondition == EmailTriggerCondition.IncompleteLab)
            {
                users = GetNotEnrolledInLabQuery(eventPortalIds);
            }
            else if (request.TriggerCondition == EmailTriggerCondition.IncompleteLabButEnrolledInSelfHelp)
            {
                users = GetNotEnrolledInLabQueryButEnrolledInSelfHelp(eventPortalIds);
            }
            else if (request.TriggerCondition == EmailTriggerCondition.WeeklyAppointmentReminder)
            {
                users = GetWeeklyAppointmentReminderQuery(eventPortalIds);
            }

            List<UserDto> usersDto = new List<UserDto>();

            foreach (DAL.User user in users)
            {
                UserDto userDto = Utility.mapper.Map<DAL.User, UserDto>(user);
                //var hra = user.HRAs.Where(x => x.Portal.Active == true).OrderByDescending(x => x.Id).FirstOrDefault();
                //if (hra != null)
                //{
                //    List<HRADto> hraList = new List<HRADto>();
                //    hraList.Add(Utility.mapper.Map<DAL.HRA,HRADto>(hra));
                //    userDto.HRAs = hraList;
                //}
                //var userinProgram = user.UsersinPrograms.Where(x => x.ProgramsinPortal.Portal.Active == true && x.IsActive == true).OrderByDescending(x => x.Id).FirstOrDefault();
                //if (userinProgram != null)
                //{
                //    List<UsersinProgramDto> usersinProgramList = new List<UsersinProgramDto>();
                //    usersinProgramList.Add(Utility.mapper.Map<DAL.UsersinProgram, UsersinProgramDto>(userinProgram));
                //    userDto.UsersinPrograms = usersinProgramList;
                //}
                if (request.TriggerCondition == EmailTriggerCondition.WeeklyAppointmentReminder)
                {
                    var appointment = user.Appointments.Where(x => x.Date > DateTime.Now && x.Active == true).OrderBy(x => x.Date).FirstOrDefault();
                    if (appointment != null)
                    {
                        //ignore if there is more than one appointment in the window
                        List<AppointmentDTO> appointmentsList = new List<AppointmentDTO>();
                        appointmentsList.Add(Utility.mapper.Map<DAL.Appointment, AppointmentDTO>(appointment));
                        userDto.Appointments = appointmentsList;
                    }
                    if (user.TimeZone != null)
                        userDto.TimeZone = user.TimeZone.TimeZoneId;
                }


                usersDto.Add(userDto);
                break;
            }
            //response.Users = Utility.mapper.Map<IList<DAL.User>, IList<UserDto>>(users.ToList());
            response.Users = usersDto;
            return response;
        }

        IQueryable<DAL.User> GetHRAQuery(IEnumerable<int> eventPortalIds)
        {
            return Context.Users.Include("Organization").Include("Organization.Portals").Include("HRAs").Where(x => !String.IsNullOrEmpty(x.Email) && (x.Complete.HasValue && x.Complete.Value) && x.Organization.Portals.Any(y => (y.HasHRA.Value == 1 || y.HasHRA == 2) && (y.HRAs.Count() == 0 || !y.HRAs.Any(z => eventPortalIds.Contains(z.PortalId) && z.CompleteDate.HasValue)) && eventPortalIds.Contains(y.Id)));
        }

        IQueryable<DAL.User> GetInCompleteProfileQuery(IEnumerable<int> eventPortalIds)
        {
            return Context.Users.Include("Organization").Include("Organization.Portals").Where(x => !String.IsNullOrEmpty(x.Email) && (!x.Complete.HasValue || !x.Complete.Value) && x.Organization.Portals.Any(y => eventPortalIds.Contains(y.Id)));
        }

        IQueryable<DAL.User> GetNotEnrolledInLabQuery(IEnumerable<int> eventPortalIds)
        {
            return Context.Users.Include("Organization").Include("Organization.Portals").Include("UsersinPrograms").Include("UsersinPrograms.ProgramsinPortal").Include("UsersinPrograms.ProgramsinPortal.Program").Where(x => !String.IsNullOrEmpty(x.Email) && (x.Complete.HasValue && x.Complete.Value) && x.Organization.Portals.Any(y => y.HasCoachingProgram && eventPortalIds.Contains(y.Id) && (y.HRAs.Count() > 0 && y.HRAs.Any(z => eventPortalIds.Contains(z.PortalId) && z.CompleteDate.HasValue)) && !x.UsersinPrograms.Any(p => p.ProgramsinPortal.Id == 2 && eventPortalIds.Contains(p.ProgramsinPortal.PortalId))));
        }

        IQueryable<DAL.User> GetNotEnrolledInLabQueryButEnrolledInSelfHelp(IEnumerable<int> eventPortalIds)
        {
            return Context.Users.Include("Organization").Include("Organization.Portals").Include("UsersinPrograms").Include("UsersinPrograms.ProgramsinPortal").Include("UsersinPrograms.ProgramsinPortal.Program").Where(x => !String.IsNullOrEmpty(x.Email) && (x.Complete.HasValue && x.Complete.Value) && x.Organization.Portals.Any(y => y.HasCoachingProgram && eventPortalIds.Contains(y.Id) && (y.HRAs.Count() > 0 && y.HRAs.Any(z => eventPortalIds.Contains(z.PortalId) && z.CompleteDate.HasValue)) && !x.UsersinPrograms.Any(p => p.ProgramsinPortal.Id == 2 && eventPortalIds.Contains(p.ProgramsinPortal.PortalId)) && x.UsersinPrograms.Any(p => p.ProgramsinPortal.Id == 1 && eventPortalIds.Contains(p.ProgramsinPortal.PortalId))));
        }

        IQueryable<DAL.User> GetWeeklyAppointmentReminderQuery(IEnumerable<int> eventPortalIds)
        {
            return Context.Users.Include("Organization").Include("Organization.Portals").Include("Appointments").Where(x => !String.IsNullOrEmpty(x.Email) && x.Organization.Portals.Any(y => eventPortalIds.Contains(y.Id)) && x.Appointments.Any(y => y.Active && y.Date >= DateTime.Now && y.Date <= DateTime.Now.AddDays(7)));
            //return Context.CreateObjectSet<DAL.User>().Include("Organization").Include("Organization.Portals").Include("Appointments").Include("TimeZone").Where(x => !String.IsNullOrEmpty(x.Email) && x.Appointments.Any(y => y.Active && y.Date >= DateTime.Now && y.Date <= DbFunctions.AddDays(DateTime.Now, 7)));
        }

        public IList<DAL.User> GetSelfHelpKitIncompletedUsers(int days, List<int> processedUserIds)
        {
            return Context.Users.Where(x => !processedUserIds.Contains(x.Id) && !string.IsNullOrEmpty(x.Email) && !x.Email.Contains("noemail.myintervent.com") && !x.Email.Contains("samlnoemail.com") && x.Organization.Portals.Any(y => y.Active && y.HasSelfHelpProgram) &&
            x.UsersinPrograms.Any(p => p.IsActive && p.ProgramsinPortal.Program.ProgramType == 1 && p.KitsinUserPrograms.Any(k => k.IsActive) &&
            (p.KitsinUserPrograms.Where(k => k.CompleteDate.HasValue).Count() == 0 && p.KitsinUserPrograms.Any(k => !k.CompleteDate.HasValue && k.StartDate.Date == DateTime.UtcNow.AddDays(days))
              || (p.KitsinUserPrograms.Where(k => k.CompleteDate.HasValue).Count() > 0 && ((p.KitsinUserPrograms.Where(k => k.CompleteDate.HasValue).OrderByDescending(o => o.CompleteDate).FirstOrDefault().CompleteDate > p.KitsinUserPrograms.Where(k => !k.CompleteDate.HasValue).OrderByDescending(o => o.StartDate).FirstOrDefault().StartDate
                  && p.KitsinUserPrograms.Where(k => k.CompleteDate.HasValue).OrderByDescending(o => o.CompleteDate).FirstOrDefault().CompleteDate.Value.Date == DateTime.UtcNow.AddDays(days).Date)
                  || (p.KitsinUserPrograms.Where(k => k.CompleteDate.HasValue).OrderByDescending(o => o.CompleteDate).FirstOrDefault().CompleteDate < p.KitsinUserPrograms.Where(k => !k.CompleteDate.HasValue).OrderByDescending(o => o.StartDate).FirstOrDefault().StartDate
                  && p.KitsinUserPrograms.Where(k => !k.CompleteDate.HasValue).OrderByDescending(o => o.StartDate).FirstOrDefault().StartDate.Date == DateTime.UtcNow.AddDays(days))))
            ))).ToList();
        }

        public IList<DAL.User> GetSelfHelpKitCompletedUsers()
        {
            int days = -1;
            return Context.Users.Where(x => !string.IsNullOrEmpty(x.Email) && !x.Email.Contains("noemail.myintervent.com") && !x.Email.Contains("samlnoemail.com") && x.Organization.Portals.Any(y => y.Active && y.HasSelfHelpProgram) &&
            x.UsersinPrograms.Any(p => p.IsActive && p.ProgramsinPortal.Program.ProgramType == 1 && p.KitsinUserPrograms.Any(k => k.IsActive && k.CompleteDate.HasValue && k.CompleteDate.Value.Date == DateTime.UtcNow.AddDays(days).Date))).ToList();
        }
    }
}
