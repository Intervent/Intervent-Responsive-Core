using Intervent.Business.Notification;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class MessageUtility
    {
        public static ListUserMessagesResponse ListUserMessages(int participantId, int userId, bool isAdmin, bool allMessages, bool ownMessage, bool drafts, bool getCount, string searchText, bool unread, bool hasPagination, int? page, int? pageSize, int? totalRecords, string timeZone, int systemAdminId)
        {
            MessageReader reader = new MessageReader();
            ListUserMessagesRequest request = new ListUserMessagesRequest();
            request.userId = participantId;
            request.adminId = userId;
            request.allMessages = allMessages;
            request.timeZone = timeZone;
            request.isAdmin = isAdmin;
            request.drafts = drafts;
            request.getCount = getCount;
            request.searchText = searchText;
            request.unread = unread;
            request.hasPagination = hasPagination;
            if (hasPagination)
            {
                request.page = page.Value;
                request.pageSize = pageSize.Value;
                request.totalRecords = totalRecords;
            }
            else
            {
                request.page = 0;
                request.pageSize = request.allMessages ? 100 : 5;
            }
            if (ownMessage)
            {
                request.coachId = userId;
            }
            request.systemAdminId = systemAdminId;
            return reader.ListUserMessages(request);
        }

        public static GetMessageDetailsResponse GetMessageDetails(int participantId, int adminId, int messageId, bool? updateStatus, bool? infoPage, string timeZone, bool isAdmin, int systemAdminId)
        {
            MessageReader reader = new MessageReader();
            GetMessageDetailsRequest request = new GetMessageDetailsRequest();
            request.messageId = messageId;
            request.userId = participantId;
            request.timeZone = timeZone;
            request.updateStatus = updateStatus.HasValue ? updateStatus.Value : true;
            if (infoPage.HasValue && infoPage.Value)
            {
                request.adminId = adminId;
            }
            request.IsAdmin = isAdmin;
            request.systemAdminId = systemAdminId;
            return reader.GetMessageDetails(request);
        }

        public static AddEditMessageResponse AddEditMessage(int messageId, int userId, int recipientId, string subject, string messageBody,
    string attachement, bool isSent, int? parentMessageId, string role, int systemAdminId)
        {
            MessageReader reader = new MessageReader();
            AddEditMessageRequest request = new AddEditMessageRequest();
            request.recipientId = recipientId;
            request.messageId = messageId;
            request.userId = userId;
            request.subject = subject;
            request.messageBody = messageBody;
            request.attachement = attachement;
            request.isSent = isSent;
            request.parentMessageId = parentMessageId;
            var response = reader.AddEditMessage(request);
            if (response.status)
            {
                if (CommonUtility.HasAdminRole(role) && isSent && recipientId != systemAdminId)
                {
                    if (NotificationUtility.hasValidEmail(recipientId))
                    {
                        NotificationUtility.CreateNewMail(NotificationEventTypeDto.NewEmail, recipientId);
                    }
                    if (new MobileReader().GetUserNotificationDevices(recipientId).Count > 0)
                    {
                        new CommonReader().AddDashboardMessage(recipientId, IncentiveMessageTypes.New_Message, null, null);
                    }
                }
            }
            return response;
        }

        public static ListFavoriteContactsResponse ListFavoriteContacts(int userId)
        {
            MessageReader reader = new MessageReader();
            ListFavoriteContactsRequest request = new ListFavoriteContactsRequest();
            request.userId = userId;
            return reader.ListFavoriteContacts(request);

        }

        public static bool AddOrRemoveFavoriteContact(int FavoriteContactId, int userId)
        {
            MessageReader reader = new MessageReader();
            AddEditFavoriteContactResponse request = new AddEditFavoriteContactResponse();
            request.UserId = userId;
            request.FavoriteContactId = FavoriteContactId;
            return reader.AddOrRemoveFavoriteContact(request);

        }

        public static MessageCountResponse GetMessageCountForDashboard(int UserId, bool IsAdmin, int? ParticipantId, bool infoPage, int systemAdminId)
        {
            MessageReader reader = new MessageReader();
            return reader.GetMessageCountForDashboard(UserId, IsAdmin, ParticipantId, infoPage, systemAdminId);
        }

        public static void DeleteAttachment(int messageId, int systemAdminId)
        {
            MessageReader reader = new MessageReader();
            DeleteAttachmentRequest request = new DeleteAttachmentRequest();
            request.MessageId = messageId;
            var response = reader.DeleteAttachment(request);
            if (response.success)
            {
                if (response.recipientId.HasValue && response.recipientId.Value != systemAdminId)
                {
                    if (NotificationUtility.hasValidEmail(response.recipientId.Value))
                    {
                        NotificationReader notificationReader = new NotificationReader();
                        var notificationEvent = notificationReader.GetNotificationEventByUserId(new GetNotificationEventByUserIdRequest() { UserId = response.recipientId.Value, NotificationEventTypeId = NotificationEventTypeDto.NewEmail.Id }).NotificationEvent.Where(x => x.NotificationStatusId == NotificationStatusDto.Queued.Id && x.NotificationEventDate > DateTime.UtcNow.AddMinutes(-10)).OrderByDescending(x => x.NotificationEventDate).FirstOrDefault();
                        if (notificationEvent != null)
                        {
                            NotificationManager _notificationManager = new NotificationManager();
                            notificationEvent.NotificationStatusId = NotificationStatusDto.Ignored.Id;
                            _notificationManager.AddOrEditNotificationEvent(new AddOrEditNotificationEventRequest() { NotificationEvent = notificationEvent });
                        }
                    }
                }
            }
        }

        public static void ActionUpdate(int messageId, bool noActionNeeded, bool markasUnread)
        {
            MessageReader reader = new MessageReader();
            ActionUpdateRequest request = new ActionUpdateRequest();
            request.MessageId = messageId;
            request.NoActionNeeded = noActionNeeded;
            request.MarkasUnread = markasUnread;
            reader.ActionUpdate(request);

        }
    }
}