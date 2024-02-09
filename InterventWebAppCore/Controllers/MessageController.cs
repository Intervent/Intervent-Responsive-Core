using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Web;

namespace InterventWebApp.Controllers
{
    public class MessageController : BaseController
    {
        private readonly IHostEnvironment environment;

        private readonly AppSettings _appSettings;

        public MessageController(IOptions<AppSettings> appSettings, IHostEnvironment environment)
        {
            _appSettings = appSettings.Value;
            this.environment = environment;
        }

        [Authorize]
        public ActionResult Messaging()
        {
            ChatMessageModel model = new ChatMessageModel();
            model.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.systemAdminId = _appSettings.SystemAdminId;
            return View(model);
        }

        [HttpPost]
        public JsonResult GetUserMessageDashboard(bool drafts, bool unread)
        {
            MessagingModel model = new MessagingModel();
            var participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value : HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            var userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            var messages = MessageUtility.ListUserMessages(participantId, userId, false, true, false, drafts, false, null, unread, false, null, null, null, HttpContext.User.TimeZone(), _appSettings.SystemAdminId);
            model.parentMessages = messages.parentMessages;
            return Json(new { Result = "OK", model });
        }

        [Authorize]
        public ActionResult GetRecentUserMessages()
        {
            MessagingModel model = new MessagingModel();
            var participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value : HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            var userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            var messages = MessageUtility.ListUserMessages(participantId, userId, false, false, false, false, false, null, false, false, null, null, null, HttpContext.User.TimeZone(), _appSettings.SystemAdminId);
            model.parentMessages = messages.parentMessages;
            model.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.systemAdminId = _appSettings.SystemAdminId;
            return PartialView("_RecentMessages", model);
        }

        [HttpPost]
        public JsonResult GetMessageDetails(int messageId, bool? updateStatus, bool? infoPage)
        {
            var participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value : HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            var userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            bool isAdmin = CommonUtility.HasAdminRole(User.RoleCode());
            var response = MessageUtility.GetMessageDetails(participantId, userId, messageId, updateStatus, infoPage, HttpContext.User.TimeZone(), isAdmin, _appSettings.SystemAdminId);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult MessageDashboard()
        {
            MessageDashboardModel model = new MessageDashboardModel();
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            model.emailUrl = _appSettings.EmailUrl;
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult GetMessageDashboard(bool isAdmin, bool ownMessage, bool drafts, bool getCount, string searchText, bool unread, bool hasPagination, int page, int pageSize, int? totalRecords)
        {
            var participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value : HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            var userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            var response = MessageUtility.ListUserMessages(participantId, userId, isAdmin, true, ownMessage, drafts, getCount, searchText, unread, hasPagination, page, pageSize, totalRecords, HttpContext.User.TimeZone(), _appSettings.SystemAdminId);
            return Json(new { Result = "OK", model = response });
        }

        [HttpPost]
        public JsonResult AddEditMessage(HttpPostedFileBase FileUpload, int messageId, int recipientId, string subject,
            string messageBody, bool isSent, int? parentMessageId, bool? infoPage)
        {
            GetMessageDetailsResponse messageDetails = new GetMessageDetailsResponse();
            string targetpath = environment.ContentRootPath + "../Messageuploads/";
            string attachement = null;
            if (FileUpload != null)
            {
                string filename = FileUpload.FileName;
                if (!string.IsNullOrEmpty(filename))
                {
                    var postedFileExtension = Path.GetExtension(FileUpload.FileName);
                    var fileName = System.DateTime.Now.ToString("_ddMMyyhhmmssFFF") + postedFileExtension;

                    if (!Directory.Exists(targetpath))
                        Directory.CreateDirectory(targetpath);
                    string pathToFile = targetpath + fileName;
                    // FileUpload.SaveAs(pathToFile);
                    attachement = fileName;
                    if (FileUpload.FileName == messageBody)
                        messageBody = fileName;
                }
            }
            var userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            var updateResponse = MessageUtility.AddEditMessage(messageId, userId, recipientId, subject, messageBody, attachement, isSent, parentMessageId, HttpContext.User.RoleCode(), _appSettings.SystemAdminId);
            if (isSent && parentMessageId.HasValue)
            {
                var participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value : HttpContext.Session.GetInt32(SessionContext.UserId).Value;
                bool isAdmin = CommonUtility.HasAdminRole(User.RoleCode());
                messageDetails = MessageUtility.GetMessageDetails(participantId, userId, updateResponse.parentMessageId, null, infoPage, HttpContext.User.TimeZone(), isAdmin, _appSettings.SystemAdminId);
            }
            return Json(new { Result = "OK", Record = messageDetails, updateResponse = updateResponse });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListFavoriteContact()
        {
            var response = MessageUtility.ListFavoriteContacts(HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            return Json(new { Result = "OK", model = response });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult AddOrRemoveFavoriteContact(int FavoriteContactId)
        {
            var response = MessageUtility.AddOrRemoveFavoriteContact(FavoriteContactId, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            return Json(new { Result = "OK", data = response });
        }

        public JsonResult DeleteAttachment(int MessageId, string AttachmentName)
        {
            System.IO.File.Delete(Path.Combine(environment.ContentRootPath, "~/Messageuploads", AttachmentName));
            MessageUtility.DeleteAttachment(MessageId, _appSettings.SystemAdminId);
            return Json(new { Result = "OK" });
        }

        [HttpPost]
        public JsonResult ActionUpdate(int messageId, bool noActionNeeded, bool markasUnread)
        {
            MessageUtility.ActionUpdate(messageId, noActionNeeded, markasUnread);
            return Json(new { Result = "OK" });
        }
    }
}