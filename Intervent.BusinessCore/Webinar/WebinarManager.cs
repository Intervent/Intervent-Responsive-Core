using Intervent.HWS;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Configuration;

namespace Intervent.Business
{
    public class WebinarManager : BaseManager
    {
        WebinarReader _webinarReader = new WebinarReader();
        public static string zoomAPIURL = ConfigurationManager.AppSettings["ZoomAPIURL"];
        public static string zoomUserId = ConfigurationManager.AppSettings["ZoomUserId"];
        public static string zoomOAuthURL = ConfigurationManager.AppSettings["ZoomOAuthURL"];
        public static string ClientId = ConfigurationManager.AppSettings["ZoomClientId"];
        public static string ClientSecret = ConfigurationManager.AppSettings["ZoomClientSecret"];
        public static string AccountId = ConfigurationManager.AppSettings["ZoomAccountId"];

        public void GetWebinarDetails()
        {
            LogReader reader = new LogReader();
            try
            {
                ZoomOAuth oAuth = Webinar.GenerateOAuthToken(ClientId, ClientSecret, zoomOAuthURL, AccountId);
                if (oAuth != null)
                {
                    GetWebinarsList(oAuth);
                    RegisteredUsersforWebinars(oAuth);
                }
                else
                {
                    var logEvent = new LogEventInfo(LogLevel.Error, "WebinarManager", null, "OAuth token was null", null, null);
                    reader.WriteLogMessage(logEvent);
                }
            }
            catch (Exception ex)
            {
                var logEvent = new LogEventInfo(LogLevel.Error, "WebinarManager", null, "Get Webinar Details", null, ex);
                reader.WriteLogMessage(logEvent);
            }
        }

        public int RegisteredUsersforWebinars(ZoomOAuth oAuth)
        {
            ListWebinarsRequest webinarsRequest = new ListWebinarsRequest() { dateFilter = true };
            var Webinars = _webinarReader.ListWebinars(webinarsRequest).webinars;
            int registeredUsers = 0;
            foreach (var webinar in Webinars)
            {
                var organizationList = webinar.OrganizationsforWebinars.Select(x => x.OrganizationId).ToList();
                if (organizationList.Count > 0)
                {
                    GetWebinarRequest request = new GetWebinarRequest();
                    request.webinarId = webinar.WebinarId;
                    request.token = oAuth.access_token;
                    request.zoomAPIURL = zoomAPIURL;
                    GetWebinarResponse response = Webinar.GetWebinarRegistrants(request);
                    if (response.registerWebinars != null && response.registerWebinars.registrants.Count > 0)
                    {
                        RegisteredUsersforWebinarsRequest registeredUsersforWebinarsRequest = new RegisteredUsersforWebinarsRequest();
                        registeredUsersforWebinarsRequest.webinarsAPIResponse = response;
                        registeredUsersforWebinarsRequest.webinarId = webinar.WebinarId;
                        registeredUsersforWebinarsRequest.organizationList = organizationList;
                        registeredUsers = registeredUsers + _webinarReader.GetRegisteredUsersforWebinars(registeredUsersforWebinarsRequest).count;
                    }
                }
            }
            if (registeredUsers > 0)
            {
                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, registeredUsers + " user(s) registered for webinars, processed on" + DateTime.Now.ToString(), null, null);
                reader.WriteLogMessage(logEvent);
            }
            return registeredUsers;
        }

        public void GetWebinarsList(ZoomOAuth oAuth)
        {
            LogReader logReader = new LogReader();
            try
            {
                GetWebinarsListResponse response = Webinar.GetWebinarsList(oAuth.access_token, zoomAPIURL, zoomUserId);
                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.webinarsList != null && response.webinarsList.webinars.Count > 0)
                {
                    var logEvent = new LogEventInfo(LogLevel.Trace, "WebinarManager", null, "Webinar details synchronized. Total : " + response.webinarsList.total_records, null, null);
                    logReader.WriteLogMessage(logEvent);
                    WebinarReader reader = new WebinarReader();
                    foreach (GetWebinarsListResponse.Webinar webinarResponse in response.webinarsList.webinars.Where(x => x.start_time > DateTime.UtcNow).ToList())
                    {
                        WebinarResponse webresponse = Webinar.GetWebinar(oAuth.access_token, webinarResponse.id, zoomAPIURL);
                        if (webresponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            WebinarResponse.Webinar webinar = webresponse.webinar;
                            WebinarDto request = new WebinarDto
                            {
                                PresentedBy = SystemAdminId,
                                HostEmail = zoomUserId,
                                WebinarId = webinar.id.ToString(),
                                HostId = webinar.host_id,
                                Agenda = webinar.agenda,
                                Duration = webinar.duration,
                                Topic = webinar.topic,
                                StartTime = webinar.start_time,
                                Type = webinar.type,
                                JoinUrl = webinar.join_url,
                                UniqueId = webinar.uuid,
                                CreatedBy = SystemAdminId,
                                CreatedOn = DateTime.UtcNow,
                                UpdatedBy = SystemAdminId,
                                UpdatedOn = DateTime.UtcNow,
                                StartUrl = webinar.start_url
                            };
                            if (!string.IsNullOrEmpty(webinar.password))
                                request.Password = webinar.password;
                            if (webinar.type == 9)
                            {
                                request.RecurrenceType = webinar.recurrence.type;
                                request.RecurrenceInterval = webinar.recurrence.repeat_interval;
                                request.RecurrenceTimes = webinar.recurrence.end_times;
                                request.StartTime = webinar.occurrences[0].start_time;
                            }
                            int webinarId = reader.AddOrEditWebinar(new AddWebinarRequest { webinar = request, systemAdminId = SystemAdminId });
                            if (webinar.type == 9)
                            {
                                foreach (WebinarResponse.Occurrence data in webinar.occurrences)
                                {
                                    WebinarOccurrenceDto occurrence = new WebinarOccurrenceDto
                                    {
                                        WebinarId = webinarId,
                                        OccurrenceId = data.occurrence_id,
                                        StartTime = data.start_time,
                                        Status = data.status,
                                        Duration = data.duration
                                    };
                                    reader.AddOrEditWebinarOccurrence(new AddOrEditWebinarOccurrenceRequest { webinarOccurrence = occurrence });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var logEvent = new LogEventInfo(LogLevel.Error, "WebinarManager", null, "Get Webinars List", null, e);
                logReader.WriteLogMessage(logEvent);
            }
        }
    }
}

