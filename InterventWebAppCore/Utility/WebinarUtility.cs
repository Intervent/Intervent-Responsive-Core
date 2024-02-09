using Intervent.HWS;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Text.Json;

namespace InterventWebApp
{
    public class WebinarUtility
    {
        public static string CreateWebinar(CreateOrUpdateWebinarModel model)
        {
            CreateWebinarRequest request = new CreateWebinarRequest
            {
                agenda = model.agenda,
                duration = model.duration,
                start_time = model.webinarDateUTC,
                password = model.password != null ? model.password : "",
                timezone = "Universal Time UTC",
                topic = model.topic,
                type = (int)WebinarTypes.Webinar
            };
            CreateWebinarResponse response = Webinar.CreateWebinar(request, Webinar.GenerateOAuthToken(model.zoomClientId, model.zoomClientSecret, model.zoomOAuthURL, model.zoomAccountId).access_token, model.zoomAPIURL, model.zoomUserId);
            LogWebinarResponse(request, response, response.Status, "Create Webinar", model.userId);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                AddWebinar(response, model.presentedBy, model.userId, model.systemAdminId);
                return "Webinar created.";
            }
            else
                return response.ErrorMsg;
        }

        public static bool AddWebinar(CreateWebinarResponse response, int presentedBy, int userId, int systemAdminId)
        {
            WebinarReader reader = new WebinarReader();
            WebinarDto webinar = new WebinarDto
            {
                Agenda = response.agenda,
                WebinarId = response.id.ToString(),
                Duration = response.duration,
                HostEmail = response.host_email,
                HostId = response.host_id,
                JoinUrl = response.join_url,
                Password = response.password,
                StartUrl = response.start_url,
                Topic = response.topic,
                Type = response.type,
                UniqueId = response.uuid,
                PresentedBy = presentedBy,
                StartTime = response.start_time,
                CreatedOn = response.created_at,
                CreatedBy = userId,
            };
            reader.AddOrEditWebinar(new AddWebinarRequest { webinar = webinar, systemAdminId = systemAdminId });
            return true;
        }

        public static string UpdateWebinar(CreateOrUpdateWebinarModel model)
        {
            UpdateWebinarRequest request = new UpdateWebinarRequest
            {
                agenda = model.agenda,
                duration = model.duration,
                start_time = model.webinarDateUTC,
                password = model.password != null ? model.password : "",
                timezone = "Universal Time UTC",
                topic = model.topic,
                type = (int)WebinarTypes.Webinar
            };
            var response = Webinar.UpdateWebinar(request, Webinar.GenerateOAuthToken(model.zoomClientId, model.zoomClientSecret, model.zoomOAuthURL, model.zoomAccountId).access_token, model.webinarId, model.zoomAPIURL);
            LogWebinarResponse(request, response, response == null || response.Status, "Update Webinar", model.userId);
            if (response == null)
            {
                EditWebinar(model);
                return "Webinar updated.";
            }
            else
                return response.ErrorMsg;
        }

        public static bool EditWebinar(CreateOrUpdateWebinarModel response)
        {
            WebinarReader reader = new WebinarReader();
            WebinarDto webinar = new WebinarDto
            {
                Id = response.id,
                Agenda = response.agenda,
                Duration = response.duration,
                Password = response.password,
                Topic = response.topic,
                StartTime = response.webinarDateUTC,
                ImageUrl = response.imageUrl,
                VideoUrl = response.videoUrl,
                Handout = response.handout,
                PresentedBy = response.presentedBy,
                UpdatedOn = DateTime.UtcNow,
                UpdatedBy = response.userId,
            };
            reader.AddOrEditWebinar(new AddWebinarRequest { webinar = webinar, systemAdminId = response.systemAdminId });
            return true;
        }

        public static GetWebinarListResponse ListWebinars(int page, int pageSize, int? totalRecords)
        {
            WebinarReader reader = new WebinarReader();
            ListWebinarsRequest request = new ListWebinarsRequest();
            request.page = page;
            request.pageSize = pageSize;
            request.totalRecords = totalRecords;
            return reader.ListWebinars(request);
        }

        public static GetWebinarsResponse GetWebinar(int id)
        {
            WebinarReader reader = new WebinarReader();
            GetWebinarsRequest request = new GetWebinarsRequest();
            request.id = id;
            return reader.GetWebinar(request);
        }

        public static AssignedWebinarsResponse ListAssignedWebinars(int webinarId)
        {
            WebinarReader reader = new WebinarReader();
            ListAssignedWebinarsRequest request = new ListAssignedWebinarsRequest();
            request.WebinarId = webinarId;
            return reader.ListAssignedWebinars(request);
        }

        public static bool AssignWebinar(int webinarId, string organizationIds)
        {
            WebinarReader reader = new WebinarReader();
            AssignedWebinarRequest request = new AssignedWebinarRequest();
            request.webinarId = webinarId;
            request.organizationIds = organizationIds.Split(',').Select(Int32.Parse).ToList();
            return reader.AssignWebinar(request);
        }

        public static bool RemoveAssignedWebinars(int Id)
        {
            WebinarReader reader = new WebinarReader();
            return reader.RemoveAssignedWebinars(Id);
        }

        public static ListUserWebinarsResponse ListUserWebinars(int orgId)
        {
            WebinarReader reader = new WebinarReader();
            return new ListUserWebinarsResponse { webinars = reader.ListUserWebinars(orgId).webinars };
        }

        public static RegisterUserForWebinarResponse RegisterUserforWebinar(string name, string emailId, string phone, int id, string webinarId, int participantId, string zoomClientId, string zoomClientSecret, string zoomOAuthURL, string zoomAccountId, string zoomAPIURL)
        {
            WebinarReader reader = new WebinarReader();
            var participantName = name.Split(' ');
            RegisterUserForWebinarRequest request = new RegisterUserForWebinarRequest { first_name = participantName[0], last_name = participantName[1], email = emailId, phone = phone };
            var response = Webinar.RegisterUserForWebinar(request, Webinar.GenerateOAuthToken(zoomClientId, zoomClientSecret, zoomOAuthURL, zoomAccountId).access_token, webinarId, zoomAPIURL);
            LogWebinarResponse(request, response, response.Status, "Register User for Webinar: " + webinarId, participantId);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                AddRegisteredUsersforWebinarsRequest webinarsRequest = new AddRegisteredUsersforWebinarsRequest();
                webinarsRequest.userId = participantId;
                webinarsRequest.webinarId = id;
                webinarsRequest.joinUrl = response.join_url;
                webinarsRequest.registrationDate = DateTime.UtcNow;
                reader.AddOrEditRegisteredUserForWebinar(webinarsRequest);
            }
            return response;
        }

        public static void LogWebinarResponse(object request, object response, bool status, string eventTitle, int userId)
        {
            LogReader reader = new LogReader();
            NLog.LogLevel logLevel = NLog.LogLevel.Info;
            if (!status)
            {
                logLevel = NLog.LogLevel.Error;
            }
            var logEvent = new LogEventInfo(logLevel, eventTitle + ". UserId : " + userId, null, " Request: " + JsonSerializer.Serialize(request) + ", Response: " + JsonSerializer.Serialize(response), null, null);
            reader.WriteLogMessage(logEvent);
        }

        public static bool AddOrEditWebinarOccurrences(string occurrenceId, string videoUrl, string handout)
        {
            WebinarReader reader = new WebinarReader();
            WebinarOccurrenceDto webinarOccurrence = new WebinarOccurrenceDto
            {
                OccurrenceId = occurrenceId,
                VideoUrl = videoUrl,
                Handout = handout
            };
            return reader.AddOrEditWebinarOccurrence(new AddOrEditWebinarOccurrenceRequest { webinarOccurrence = webinarOccurrence });
        }
    }
}
