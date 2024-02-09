using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class MessageReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public ListUserMessagesResponse ListUserMessages(ListUserMessagesRequest request)
        {
            StoredProcedures sp = new StoredProcedures();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timeZone);
            ListUserMessagesResponse response = new ListUserMessagesResponse();
            response.parentMessages = new List<MessageDto>();
            List<GetMessages_Result> result = sp.GetMessages(request.isAdmin, request.searchText, request.drafts, request.userId, request.systemAdminId, request.coachId, request.unread, request.page, request.pageSize, request.totalRecords);

            if (result.Count() > 0)
            {
                if (request.getCount)
                    response.messageCount = GetMessageCountForDashboard(request.adminId, true, null, false, request.systemAdminId);
                response.totalRecords = result.Select(x => x.Total.Value).FirstOrDefault();
                request.userId = !request.isAdmin ? request.adminId : request.userId;
                foreach (GetMessages_Result msg in result)
                {
                    MessageDto newMsg = new MessageDto();
                    newMsg.Id = msg.MsgId;
                    newMsg.ParentMessageId = msg.ParentMessageId;
                    newMsg.CreateDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(msg.CreateDate.ToString()), custTZone);
                    newMsg.Subject = msg.Subject;
                    newMsg.MessageBody = msg.MessageBody;
                    newMsg.hasAttachment = msg.Attachment != null;
                    newMsg.RecentMessage = msg.RecentMessage;
                    newMsg.NoActionNeeded = msg.NoActionNeeded;
                    newMsg.IsRead = msg.IsRead.HasValue ? msg.IsRead.Value : false;
                    newMsg.IsSent = msg.IsSent;
                    newMsg.NoActionNeeded = msg.NoActionNeeded;
                    newMsg.LastMessageDate = msg.LastMessageDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(msg.LastMessageDate.ToString()), custTZone) : TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(msg.CreateDate.ToString()), custTZone);
                    newMsg.CanShowDelete = false;
                    if (msg.HasResentAttachment != null && TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone) <= msg.LastMessageDate.Value.AddMinutes(10) && msg.MessageCreatorId == request.userId)
                        newMsg.CanShowDelete = true;
                    newMsg.SeenBy = "";

                    if ((((request.isAdmin || (request.adminId != msg.MessageCreatorId)) && msg.RecipientId == request.systemAdminId) || msg.RecipientId == request.userId) && !msg.ReadById.HasValue)
                    {
                        newMsg.StatusId = 1;//new
                        newMsg.CanShowUnread = true;
                    }
                    else if ((msg.RecipientId == request.userId && msg.ReadById.HasValue))
                        newMsg.StatusId = 3;//seen 
                    else if (msg.RecipientId != request.userId && !msg.ReadById.HasValue)
                        newMsg.StatusId = 2;//sent
                    else if (msg.RecipientId != request.userId && msg.ReadById.HasValue)
                        newMsg.StatusId = 3;//seen
                    if (newMsg.StatusId == 3 && msg.ReadByName != null)
                    {
                        newMsg.SeenBy = "by " + msg.ReadByName;
                        if (msg.ReadById == request.userId)
                            newMsg.CanShowUnread = true;
                    }

                    newMsg.CreatorName = msg.FirstName + " " + msg.LastName;
                    if (!string.IsNullOrEmpty(msg.Picture))
                        newMsg.Picture = msg.Picture;
                    else
                        newMsg.Picture = msg.Gender == 1 ? "avatar-male.svg" : "avatar-female.svg";

                    newMsg.CreatorRole = msg.CreatorRole != null ? msg.CreatorRole : "Participant";
                    response.parentMessages.Add(newMsg);
                }
                response.parentMessages = response.parentMessages.OrderByDescending(x => x.LastMessageDate).ToList();
            }
            return response;
        }

        public DeleteAttachmentResponse DeleteAttachment(DeleteAttachmentRequest request)
        {
            DeleteAttachmentResponse response = new DeleteAttachmentResponse();
            var messageRecipientEntry = context.MessageRecipients.Where(x => x.MessageId == request.MessageId).FirstOrDefault();
            if (messageRecipientEntry != null)
            {
                response.recipientId = messageRecipientEntry.RecipientId;
                response.success = true;
                context.MessageRecipients.Remove(messageRecipientEntry);
                context.SaveChanges();
            }
            var messagesEntry = context.Messages.Where(x => x.Id == request.MessageId).FirstOrDefault();
            if (messagesEntry != null)
            {
                context.Messages.Remove(messagesEntry);
                context.SaveChanges();
            }
            return response;
        }

        public void ActionUpdate(ActionUpdateRequest request)
        {
            if (request.NoActionNeeded)
            {
                var message = context.Messages.Where(x => x.Id == request.MessageId).FirstOrDefault();
                if (message != null)
                {
                    message.NoActionNeeded = request.NoActionNeeded;
                    message.CaseClosedDate = DateTime.UtcNow;
                    context.Messages.Attach(message);
                    context.Entry(message).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else
            {
                var messageRecipient = context.MessageRecipients.Where(x => x.MessageId == request.MessageId).FirstOrDefault();
                if (messageRecipient != null)
                {
                    messageRecipient.IsRead = false;
                    messageRecipient.ReadBy = null;
                    context.MessageRecipients.Attach(messageRecipient);
                    context.Entry(messageRecipient).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public AddEditMessageResponse AddEditMessage(AddEditMessageRequest request)
        {
            AddEditMessageResponse response = new AddEditMessageResponse();
            if (request.messageId == 0)
            {
                response = AddMessage(request);
            }
            else
            {
                var message = context.Messages.Where(x => x.Id == request.messageId).FirstOrDefault();
                var lastestMessageId = context.Messages.OrderByDescending(x => x.Id).FirstOrDefault().Id;
                if (String.IsNullOrEmpty(request.messageBody) || (!message.IsSent && request.isSent && lastestMessageId != message.Id))
                {
                    var messageRecipient = context.MessageRecipients.Where(x => x.MessageId == request.messageId.Value).FirstOrDefault();
                    context.MessageRecipients.Remove(messageRecipient);
                    context.Messages.Remove(message);
                    response.updatedId = 0;
                    context.SaveChanges();
                    if (!message.IsSent && request.isSent)
                    {
                        if (request.parentMessageId == request.messageId)
                        {
                            request.messageId = 0;
                            request.parentMessageId = null;
                        }
                        response = AddMessage(request);
                    }
                }
                else
                {
                    message.Subject = request.subject;
                    message.MessageBody = request.messageBody;
                    message.IsSent = request.isSent;
                    message.CreateDate = DateTime.UtcNow;
                    if (!String.IsNullOrEmpty(request.attachement))
                        message.Attachment = request.attachement;
                    context.Messages.Attach(message);
                    context.Entry(message).State = EntityState.Modified;
                    response.updatedId = message.Id;
                    response.parentMessageId = message.ParentMessageId.Value;

                    var messageRecipient = context.MessageRecipients.Where(x => x.MessageId == request.messageId.Value).FirstOrDefault();
                    if (messageRecipient.RecipientId != request.recipientId)
                    {
                        messageRecipient.RecipientId = request.recipientId;
                        context.MessageRecipients.Attach(messageRecipient);
                        context.Entry(messageRecipient).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            }
            response.status = true;
            return response;
        }

        public AddEditMessageResponse AddMessage(AddEditMessageRequest request)
        {
            AddEditMessageResponse response = new AddEditMessageResponse();
            DAL.Message message = new DAL.Message();
            message.Subject = request.subject;
            message.MessageBody = request.messageBody;
            message.CreatorId = request.userId;
            message.IsSent = request.isSent;
            if (!String.IsNullOrEmpty(request.attachement))
                message.Attachment = request.attachement;
            message.CreateDate = DateTime.UtcNow;
            message.ParentMessageId = request.parentMessageId;
            context.Messages.Add(message);
            context.SaveChanges();
            if (request.parentMessageId == null)
                UpdateMessage(message);
            DAL.MessageRecipient messageRecipients = new DAL.MessageRecipient();
            messageRecipients.RecipientId = request.recipientId;
            messageRecipients.MessageId = response.updatedId = message.Id;
            context.MessageRecipients.Add(messageRecipients);
            context.SaveChanges();
            response.parentMessageId = message.ParentMessageId.HasValue ? message.ParentMessageId.Value : message.Id;
            response.status = true;
            return response;
        }

        public void UpdateMessage(Message request)
        {
            request.ParentMessageId = request.Id;
            context.Messages.Attach(request);
            context.Entry(request).State = EntityState.Modified;
            context.SaveChanges();
        }

        public GetMessageDetailsResponse GetMessageDetails(GetMessageDetailsRequest request)
        {
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timeZone);
            GetMessageDetailsResponse response = new GetMessageDetailsResponse();
            var messages = context.Messages.Include("User").Include("User.UserRoles").Include("MessageRecipients").Include("MessageRecipients.User").Include("MessageRecipients.User.UserRoles").Where(x => x.Id == request.messageId || x.ParentMessageId == request.messageId).OrderBy(x => x.CreateDate).ToList();
            response.Messages = new List<MessageDto>();
            response.Messages = ProcessMessageDetails(messages, request.userId, request.updateStatus, custTZone, true, request.adminId, request.IsAdmin, request.systemAdminId);
            return response;
        }

        public void UpdateIsRead(int[] MessageRecipientIds, int ReadedBy)
        {
            foreach (var MessageRecipientId in MessageRecipientIds)
            {
                var messageRecipient = context.MessageRecipients.Where(x => x.Id == MessageRecipientId).FirstOrDefault();
                messageRecipient.IsRead = true;
                messageRecipient.ReadBy = ReadedBy;
                context.MessageRecipients.Attach(messageRecipient);
                context.Entry(messageRecipient).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public List<MessageDto> ProcessMessageDetails(IList<Message> messageDetails, int userId, bool updateStatus, TimeZoneInfo custTZone, bool isRead, int? adminId, bool isAdmin, int systemAdminId)
        {
            var messageDetailsDto = new List<MessageDto>();
            if (messageDetails != null && messageDetails.Count() > 0)
            {
                foreach (var messageDetail in messageDetails)
                {
                    if (messageDetail.Id == messageDetails[messageDetails.Count - 1].Id)
                    {
                    }
                    MessageDto messageDetailDto = new MessageDto();
                    messageDetailDto.CreateDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(messageDetail.CreateDate.ToString()), custTZone);
                    messageDetailDto.Subject = messageDetail.Subject;
                    messageDetailDto.MessageBody = messageDetail.MessageBody;
                    messageDetailDto.NoActionNeeded = messageDetail.NoActionNeeded;
                    messageDetailDto.IsSent = messageDetail.IsSent;
                    messageDetailDto.Id = messageDetail.Id;
                    messageDetailDto.Attachment = messageDetail.Attachment;
                    messageDetailDto.DisplayName = messageDetail.User.FirstName + " " + messageDetail.User.LastName;
                    messageDetailDto.DisplayId = messageDetail.CreatorId;
                    if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone) < messageDetailDto.CreateDate.AddMinutes(10) && messageDetail.MessageRecipients.FirstOrDefault().ReadBy == null && (messageDetail.CreatorId == userId || (adminId.HasValue && messageDetail.CreatorId == adminId)))
                        messageDetailDto.CanShowDelete = true;
                    else
                        messageDetailDto.CanShowDelete = false;
                    if (messageDetail.MessageRecipients.FirstOrDefault().RecipientId == systemAdminId)
                        messageDetailDto.CreatorId = messageDetail.CreatorId;
                    else if (isAdmin || messageDetail.MessageRecipients.FirstOrDefault().RecipientId != userId)
                        messageDetailDto.CreatorId = messageDetail.MessageRecipients.FirstOrDefault().RecipientId.Value;
                    else
                        messageDetailDto.CreatorId = systemAdminId;
                    if (userId != messageDetail.CreatorId)
                    {
                        messageDetailDto.CreatorName = messageDetail.User.FirstName + " " + messageDetail.User.LastName;
                        if (messageDetail.User.UserRoles != null && messageDetail.User.UserRoles.Count() > 0)
                        {
                            messageDetailDto.CreatorRole = messageDetail.User.UserRoles.FirstOrDefault().Name;
                        }
                    }
                    messageDetailDto.MessageRecipients = new List<MessageRecipientDto>();
                    if (messageDetail.MessageRecipients != null && messageDetail.MessageRecipients.Count() > 0)
                    {
                        MessageRecipientDto recipientDto = new MessageRecipientDto();
                        var recipient = messageDetail.MessageRecipients.Where(x => x.MessageId == messageDetail.Id).FirstOrDefault();
                        recipientDto.IsRead = recipient.IsRead;
                        recipientDto.RecipientId = recipient.RecipientId;
                        recipientDto.RecipientName = recipient.User.FirstName + " " + recipient.User.LastName;
                        if (recipient.RecipientId != systemAdminId)
                        {
                            if (!string.IsNullOrEmpty(recipient.User.Picture))
                                recipientDto.OwnershipImage = recipient.User.Picture;
                            else
                                recipientDto.OwnershipImage = CommonReader.GetGenderSpecificImage(recipient.User);
                            recipientDto.OwnershipName = recipient.User.FirstName + " " + recipient.User.LastName;
                            if (recipient.User.UserRoles != null && recipient.User.UserRoles.Count() > 0)
                            {
                                recipientDto.OwnershipRole = recipient.User.UserRoles.FirstOrDefault().Name;
                            }
                            else
                                recipientDto.OwnershipRole = "Participant";
                            recipientDto.OwnershipId = recipient.User.Id;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(messageDetail.User.Picture))
                                recipientDto.OwnershipImage = messageDetail.User.Picture;
                            else
                                recipientDto.OwnershipImage = CommonReader.GetGenderSpecificImage(messageDetail.User);
                            recipientDto.OwnershipName = messageDetail.User.FirstName + " " + messageDetail.User.LastName;
                            if (messageDetail.User.UserRoles != null && messageDetail.User.UserRoles.Count() > 0)
                            {
                                recipientDto.OwnershipRole = messageDetail.User.UserRoles.FirstOrDefault().Name;
                            }
                            else
                                recipientDto.OwnershipRole = "Participant";
                            recipientDto.OwnershipId = messageDetail.User.Id;
                        }
                        messageDetailDto.MessageRecipients.Add(recipientDto);
                        //if (updateStatus && isRead && (messageDetail.CreatorId != userId || (adminId.HasValue && adminId != messageDetail.CreatorId)) && messageDetail.IsSent && (isAdmin == false || messageDetail.User.UserRoles == null || messageDetail.User.UserRoles.Count == 0 || (isAdmin && messageDetail.CreatorId != userId)))
                        if (updateStatus && isRead && (messageDetail.CreatorId != userId || (adminId.HasValue && adminId != messageDetail.CreatorId)) && messageDetail.IsSent && (isAdmin == false || messageDetail.User.UserRoles == null || messageDetail.User.UserRoles.Count == 0 || (isAdmin && (messageDetail.User.UserRoles.Count > 0 && messageDetail.MessageRecipients.FirstOrDefault().User.UserRoles.Count() > 0) && (((!adminId.HasValue && userId != messageDetail.CreatorId) || adminId != messageDetail.CreatorId)))))
                        {
                            int[] messageRecipientIds = messageDetail.MessageRecipients.Where(x => x.IsRead == false).Select(x => x.Id).ToArray();
                            if (messageRecipientIds.Count() > 0)
                            {
                                UpdateIsRead(messageRecipientIds, adminId.HasValue ? adminId.Value : userId);
                                messageDetailDto.StatusChange = true;
                                messageDetailDto.NewStatus = 1;
                            }
                        }
                    }
                    if (isAdmin && (messageDetail.User.UserRoles.Count > 0 && messageDetail.MessageRecipients.FirstOrDefault().User.UserRoles.Count() > 0) && (((!adminId.HasValue && userId != messageDetail.CreatorId) || (adminId.HasValue && adminId != messageDetail.CreatorId))))
                    {
                        messageDetailDto.CreatorId = messageDetail.CreatorId;
                    }
                    messageDetailsDto.Add(messageDetailDto);

                }
            }
            return messageDetailsDto;
        }

        public MessageCountResponse GetMessageCountForDashboard(int UserId, bool IsAdmin, int? ParticipantId, bool infoPage, int systemAdminId)
        {
            MessageCountResponse messageCount = new MessageCountResponse();
            var organizationsList = new int?[] { };
            PortalReader reader = new PortalReader();
            if (IsAdmin)
                organizationsList = reader.GetFilteredOrganizationsList(UserId).Organizations.Select(x => x.Id).ToArray();
            var userAllMessages = context.Messages.Include("MessageRecipients").Where(x => (((IsAdmin || infoPage) && x.IsSent && ((x.MessageRecipients.Any(y => y.IsRead == false && (y.RecipientId == systemAdminId)))
                         || (x.CreatorId != UserId && x.MessageRecipients.Any(z => z.IsRead == false && z.RecipientId == UserId)))
                         && (!infoPage || x.CreatorId == ParticipantId))
                         || (!IsAdmin && x.IsSent && x.MessageRecipients.Any(y => y.RecipientId == UserId && y.IsRead == false)))
                         && ((!IsAdmin) || (IsAdmin && ParticipantId.HasValue) || (organizationsList.Count() != 0 && organizationsList.Contains(x.User.OrganizationId))))
                         .GroupBy(x => x.ParentMessageId.HasValue ? x.ParentMessageId : x.Id).Select(y => y.FirstOrDefault().ParentMessageId.HasValue ? y.FirstOrDefault().ParentMessageId.Value : y.FirstOrDefault().Id).ToList();
            if (IsAdmin)
            {
                ParticipantId = ParticipantId.HasValue ? ParticipantId.Value : UserId;
                messageCount.MyMessagesCount = context.Messages.Where(x => (userAllMessages.Contains(x.Id) || (x.ParentMessageId.HasValue && userAllMessages.Contains(x.ParentMessageId.Value))) && (x.CreatorId == ParticipantId || x.MessageRecipients.Any(y => y.RecipientId == ParticipantId)))
                         .GroupBy(x => x.ParentMessageId.HasValue ? x.ParentMessageId : x.Id).ToList().Count();
            }
            messageCount.MessageBoardCount = userAllMessages.Count();
            return messageCount;

        }

        public ListFavoriteContactsResponse ListFavoriteContacts(ListFavoriteContactsRequest request)
        {
            PortalReader reader = new PortalReader();
            ListFavoriteContactsResponse response = new ListFavoriteContactsResponse();
            var contactIds = context.FavoriteContacts.Where(x => x.UserId == request.userId).Select(x => x.FavoriteContactId).ToList();
            var contacts = context.Users.Include("UserRoles").Where(x => contactIds.Contains(x.Id));
            response = new ListFavoriteContactsResponse();
            response.favoriteContacts = new List<FavoriteContactDto>();
            List<FavoriteContact> lstContact = new List<FavoriteContact>();
            foreach (var user in contacts)
            {
                FavoriteContactDto favoriteContact = new FavoriteContactDto();
                favoriteContact.Id = user.Id;
                if (!string.IsNullOrEmpty(user.Picture))
                    favoriteContact.Picture = user.Picture;
                else
                    favoriteContact.Picture = CommonReader.GetGenderSpecificImage(user);

                if (user.UserRoles != null && user.UserRoles.Count() > 0)
                {
                    favoriteContact.RoleName = user.UserRoles.FirstOrDefault().Name;
                }
                else
                    favoriteContact.RoleName = "Participant";
                favoriteContact.ContactName = user.FirstName + " " + user.LastName;
                response.favoriteContacts.Add(favoriteContact);
            }
            return response;
        }

        public bool AddOrRemoveFavoriteContact(AddEditFavoriteContactResponse request)
        {
            bool isAdd = false;
            DAL.FavoriteContact favoriteContact = new FavoriteContact();
            var favContact = context.FavoriteContacts.Where(x => x.UserId == request.UserId && x.FavoriteContactId == request.FavoriteContactId).FirstOrDefault();

            if (favContact == null)
            {
                isAdd = true;
                favoriteContact.UserId = request.UserId;
                favoriteContact.FavoriteContactId = request.FavoriteContactId;
                context.FavoriteContacts.Add(favoriteContact);
            }
            else
            {
                context.FavoriteContacts.Remove(favContact);
            }
            context.SaveChanges();
            return isAdd;
        }
    }
}
