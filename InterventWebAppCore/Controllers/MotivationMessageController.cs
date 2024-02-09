using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InterventWebApp
{
    public class MotivationMessageController : AccountBaseController
    {
        private readonly AppSettings _appSettings;

        public MotivationMessageController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public ActionResult MotivationMessage()
        {
            ViewData["BaseUrl"] = _appSettings.EmailUrl;
            return View();
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public JsonResult ListAssignedMotivationMessages(int motivationMessageId)
        {
            var assignedMessageList = MotivationMessageUtility.ListAssignedMotivationMessages(motivationMessageId).assignedMotivationMessages;
            foreach (var assignedMessage in assignedMessageList)
            {
                var messageType = assignedMessage.MessageType.Split(',');
                for (var i = 0; i < messageType.Length; i++)
                {
                    if (messageType[i] == "1")
                        messageType[Array.IndexOf(messageType, "1")] = "Email";

                    if (messageType[i] == "2")
                        messageType[Array.IndexOf(messageType, "2")] = "SMS";

                    if (messageType[i] == "3")
                        messageType[Array.IndexOf(messageType, "3")] = "Dashboard";
                }

                string newMessageType = string.Join(", ", messageType);

                assignedMessage.MessageType = newMessageType;
            }

            var result = assignedMessageList;

            return Json(new { Result = "OK", Records = result });
        }

        [ModuleControl(null, RoleCode.Administrator)]
        [HttpPost]
        public JsonResult AddEditMotivationMessage(MotivationMessageModel model)
        {
            var response = MotivationMessageUtility.AddEditMotivationMessage(model);
            return Json(new { Record = response });
        }

        [ModuleControl(null, RoleCode.Administrator)]
        [HttpPost]
        public JsonResult AssignOrRemoveMotivationMessage(int MessageId, int? AssignedMessageId, string OrganizationIds, string MessageTypes, bool isRemove)
        {
            var response = MotivationMessageUtility.AssignOrRemoveMotivationMessage(MessageId, AssignedMessageId, OrganizationIds, MessageTypes, isRemove);
            return Json(new { Record = response });
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public JsonResult ListMotivationMessages(int page, int pageSize, int? totalRecords)
        {
            var response = MotivationMessageUtility.ListMotivationMessages(page, pageSize, totalRecords);
            return Json(new { Result = "OK", Records = response.MotivationMessages, TotalRecords = response.TotalRecords });
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public JsonResult GetMotivationMessage(int id)
        {
            var response = MotivationMessageUtility.GetMotivationMessage(id);
            return Json(new { Result = "OK", Record = response.MotivationMessage });
        }

    }
}