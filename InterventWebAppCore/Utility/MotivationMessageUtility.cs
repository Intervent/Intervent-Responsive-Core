using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class MotivationMessageUtility
    {
        public static AddEditMotivationMessageResponse AddEditMotivationMessage(MotivationMessageModel model)
        {
            MotivationMessageReader reader = new MotivationMessageReader();
            AddEditMotivationMessageRequest request = new AddEditMotivationMessageRequest();
            MotivationMessagesDto motivationMessage = new MotivationMessagesDto();
            motivationMessage.Id = model.Id;
            motivationMessage.MessageContent = model.MessageContent;
            motivationMessage.Subject = model.Subject;
            request.motivationMessage = motivationMessage;

            return reader.AddEditMotivationMessage(request);
        }

        public static bool AssignOrRemoveMotivationMessage(int MessageId, int? AssignedMessageId, string OrganizationIds, string MessageTypes, bool isRemove)
        {
            MotivationMessageReader reader = new MotivationMessageReader();
            AssignMotivationMessagesRequest assignMessagesRequest = new AssignMotivationMessagesRequest();
            assignMessagesRequest.MessageId = MessageId;
            assignMessagesRequest.AssignedMessageId = AssignedMessageId;
            assignMessagesRequest.MessageTypes = MessageTypes;
            assignMessagesRequest.OrganizationIds = OrganizationIds;
            assignMessagesRequest.isRemove = isRemove;
            return reader.AssignOrRemoveMotivationMessage(assignMessagesRequest);
        }

        public static ListMotivationMessagesResponse ListMotivationMessages(int page, int pageSize, int? totalRecords)
        {
            MotivationMessageReader reader = new MotivationMessageReader();
            ListMotivationMessagesRequest motivationMessageRequest = new ListMotivationMessagesRequest();
            motivationMessageRequest.Page = page;
            motivationMessageRequest.PageSize = pageSize;
            motivationMessageRequest.TotalRecords = totalRecords;

            return reader.ListMotivationMessages(motivationMessageRequest);
        }


        public static GetMotivationMessageResponse GetMotivationMessage(int id)
        {
            MotivationMessageReader reader = new MotivationMessageReader();
            GetMotivationMessageRequest motivationMessageRequest = new GetMotivationMessageRequest();
            motivationMessageRequest.Id = id;
            return reader.GetMotivationMessage(motivationMessageRequest);
        }

        public static ListAssignedMotivationMessagesResponse ListAssignedMotivationMessages(int motivationMessageId)
        {
            MotivationMessageReader reader = new MotivationMessageReader();
            ListAssignedMotivationMessagesRequest request = new ListAssignedMotivationMessagesRequest();
            request.motivationMessageId = motivationMessageId;
            return reader.ListAssignedMotivationMessages(request);
        }
    }
}