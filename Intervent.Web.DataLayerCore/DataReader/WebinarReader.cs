using Intervent.DAL;
using Intervent.HWS;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class WebinarReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public GetWebinarListResponse ListWebinars(ListWebinarsRequest request)
        {
            GetWebinarListResponse response = new GetWebinarListResponse();
            IList<DAL.Webinar> webinars;
            if (request.dateFilter)
            {
                webinars = context.Webinars.Include("OrganizationsforWebinars").Include("OrganizationsforWebinars.Organization").Where(x => x.StartTime >= DateTime.UtcNow).ToList();
            }
            else
            {
                response.totalRecords = request.totalRecords.HasValue && request.totalRecords.Value != 0 ? request.totalRecords.Value : context.Webinars.Count();
                webinars = context.Webinars.Include("User").OrderByDescending(x => x.StartTime).Skip(request.page * request.pageSize).Take(request.pageSize).ToList();
            }
            response.webinars = Utility.mapper.Map<IList<DAL.Webinar>, IList<WebinarDto>>(webinars);
            return response;
        }

        public GetWebinarsResponse GetWebinar(GetWebinarsRequest request)
        {
            GetWebinarsResponse response = new GetWebinarsResponse();
            var webinar = context.Webinars.Include("WebinarOccurrences").Include("User").Where(x => x.Id == request.id).FirstOrDefault();
            response.webinar = Utility.mapper.Map<DAL.Webinar, WebinarDto>(webinar);
            return response;
        }

        public int AddOrEditWebinar(AddWebinarRequest request)
        {
            var webinar = context.Webinars.Where(x => x.Id == request.webinar.Id || x.WebinarId == request.webinar.WebinarId).FirstOrDefault();
            if (webinar == null)
            {
                webinar = Utility.mapper.Map<WebinarDto, DAL.Webinar>(request.webinar);
                context.Webinars.Add(webinar);
            }
            else
            {
                if (request.webinar.PresentedBy.HasValue && request.webinar.PresentedBy.Value != request.systemAdminId)
                    webinar.PresentedBy = request.webinar.PresentedBy;
                if (request.webinar.HostEmail != null)
                    webinar.HostEmail = request.webinar.HostEmail;
                if (request.webinar.Agenda != null)
                    webinar.Agenda = request.webinar.Agenda;
                if (request.webinar.Duration != null)
                    webinar.Duration = request.webinar.Duration;
                if (request.webinar.StartUrl != null)
                    webinar.StartUrl = request.webinar.StartUrl;
                if (request.webinar.StartTime != null)
                    webinar.StartTime = request.webinar.StartTime;
                if (request.webinar.UniqueId != null)
                    webinar.UniqueId = request.webinar.UniqueId;
                if (request.webinar.Password != null)
                    webinar.Password = request.webinar.Password;
                if (request.webinar.Topic != null)
                    webinar.Topic = request.webinar.Topic;
                if (request.webinar.JoinUrl != null)
                    webinar.JoinUrl = request.webinar.JoinUrl;
                if (request.webinar.ImageUrl != null)
                    webinar.ImageUrl = request.webinar.ImageUrl;
                if (request.webinar.VideoUrl != null)
                    webinar.VideoUrl = request.webinar.VideoUrl;
                if (request.webinar.Handout != null)
                    webinar.Handout = request.webinar.Handout;
                if (request.webinar.Type != null)
                    webinar.Type = request.webinar.Type;
                webinar.UpdatedBy = request.webinar.UpdatedBy;
                webinar.UpdatedOn = request.webinar.UpdatedOn;
                context.Webinars.Attach(webinar);
                context.Entry(webinar).State = EntityState.Modified;
            }
            context.SaveChanges();
            return webinar.Id;
        }

        public bool AddOrEditWebinarOccurrence(AddOrEditWebinarOccurrenceRequest request)
        {
            var webinar = context.WebinarOccurrences.Where(x => x.OccurrenceId == request.webinarOccurrence.OccurrenceId).FirstOrDefault();
            if (webinar == null)
            {
                webinar = Utility.mapper.Map<WebinarOccurrenceDto, DAL.WebinarOccurrence>(request.webinarOccurrence);
                context.WebinarOccurrences.Add(webinar);
            }
            else
            {
                if (request.webinarOccurrence.StartTime != null && request.webinarOccurrence.StartTime != DateTime.MinValue)
                    webinar.StartTime = request.webinarOccurrence.StartTime;
                if (request.webinarOccurrence.Status != null)
                    webinar.Status = request.webinarOccurrence.Status;
                if (request.webinarOccurrence.Duration != 0)
                    webinar.Duration = request.webinarOccurrence.Duration;
                if (request.webinarOccurrence.VideoUrl != null)
                    webinar.VideoUrl = request.webinarOccurrence.VideoUrl;
                if (request.webinarOccurrence.Handout != null)
                    webinar.Handout = request.webinarOccurrence.Handout;
                context.WebinarOccurrences.Attach(webinar);
                context.Entry(webinar).State = EntityState.Modified;
            }
            context.SaveChanges();
            return true;
        }

        public bool AssignWebinar(AssignedWebinarRequest request)
        {
            if (request.organizationIds.Count > 0)
            {
                foreach (int orgId in request.organizationIds)
                {
                    if (context.OrganizationsforWebinars.Where(x => x.WebinarId == request.webinarId && x.OrganizationId == orgId).FirstOrDefault() == null)
                    {
                        context.OrganizationsforWebinars.Add(new OrganizationsforWebinar()
                        {
                            WebinarId = request.webinarId,
                            OrganizationId = orgId
                        });
                        context.SaveChanges();
                    }
                }
            }
            return true;
        }

        public bool RemoveAssignedWebinars(int Id)
        {
            var organizationsforWebinar = context.OrganizationsforWebinars.Where(x => x.Id == Id).FirstOrDefault();
            if (organizationsforWebinar != null)
            {
                context.OrganizationsforWebinars.Remove(organizationsforWebinar);
                context.SaveChanges();
            }
            return true;
        }

        public ListUserWebinarsResponse ListUserWebinars(int orgId)
        {
            ListUserWebinarsResponse response = new ListUserWebinarsResponse();
            var organizationsforWebinars = context.OrganizationsforWebinars.Include("Organization").Include("Webinar").Include("Webinar.WebinarOccurrences").Include("Webinar.User").Include("Webinar.RegisteredUsersforWebinars").Include("Webinar.User.AdminProperty").Where(x => x.OrganizationId == orgId).ToList();
            response.webinars = Utility.mapper.Map<IList<DAL.OrganizationsforWebinar>, IList<OrganizationsforWebinarDto>>(organizationsforWebinars);
            return response;
        }

        public AssignedWebinarsResponse ListAssignedWebinars(ListAssignedWebinarsRequest request)
        {
            AssignedWebinarsResponse response = new AssignedWebinarsResponse();
            var organizationsforWebinars = context.OrganizationsforWebinars.Include("Organization").Where(x => x.WebinarId == request.WebinarId).ToList();
            response.webinars = Utility.mapper.Map<IList<DAL.OrganizationsforWebinar>, IList<OrganizationsforWebinarDto>>(organizationsforWebinars);
            return response;
        }

        public RegisteredUsersforWebinarsResponse GetRegisteredUsersforWebinars(RegisteredUsersforWebinarsRequest request)
        {
            AccountReader accountReader = new AccountReader();
            RegisteredUsersforWebinarsResponse response = new RegisteredUsersforWebinarsResponse();
            response.count = 0;
            foreach (var registrants in request.webinarsAPIResponse.registerWebinars.registrants)
            {
                var user = accountReader.GetUserByUserName(registrants.email);
                if (user != null && request.organizationList.Contains(user.OrganizationId))
                {
                    int webinarId = context.Webinars.Where(x => x.WebinarId == request.webinarId).FirstOrDefault().Id;
                    AddRegisteredUsersforWebinarsRequest webinarsRequest = new AddRegisteredUsersforWebinarsRequest();
                    webinarsRequest.userId = user.Id;
                    webinarsRequest.webinarId = webinarId;
                    webinarsRequest.registrationDate = Convert.ToDateTime(registrants.create_time);
                    webinarsRequest.joinUrl = registrants.join_url;
                    AddOrEditRegisteredUserForWebinar(webinarsRequest);
                    response.count = response.count + 1;
                }
            }
            return response;
        }


        public bool AddOrEditRegisteredUserForWebinar(AddRegisteredUsersforWebinarsRequest request)
        {
            AccountReader accountReader = new AccountReader();
            ExternalReader externalReader = new ExternalReader();
            var registeredUsers = context.RegisteredUsersforWebinars.Where(x => x.UserId == request.userId && x.WebinarId == request.webinarId).FirstOrDefault();
            if (registeredUsers == null)
            {
                RegisteredUsersforWebinar dal = new RegisteredUsersforWebinar
                {
                    UserId = request.userId,
                    WebinarId = request.webinarId,
                    RegistrationDate = request.registrationDate,
                    UserJoinUrl = request.joinUrl
                };
                context.RegisteredUsersforWebinars.Add(dal);
                context.SaveChanges();
            }
            else if (!string.IsNullOrEmpty(request.joinUrl) && (string.IsNullOrEmpty(registeredUsers.UserJoinUrl) || registeredUsers.UserJoinUrl != request.joinUrl))
            {
                registeredUsers.UserJoinUrl = request.joinUrl;
                context.RegisteredUsersforWebinars.Attach(registeredUsers);
                context.Entry(registeredUsers).State = EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }
    }
}
