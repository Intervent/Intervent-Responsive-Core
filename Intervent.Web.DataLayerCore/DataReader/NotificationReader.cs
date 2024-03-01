using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class NotificationReader : BaseDataReader
    {
        InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        static Dictionary<int, GetNotificationTemplateResponse> dictNotificationTemplate = new Dictionary<int, GetNotificationTemplateResponse>();
        public AddOrEditNotificationEventResponse AddOrEditNotificationEvent(AddOrEditNotificationEventRequest request)
        {
            //map to DAL and insert

            DAL.NotificationEvent notifyEvent = new DAL.NotificationEvent();
            notifyEvent.BccAddress = request.NotificationEvent.BccAddress;
            notifyEvent.CcAddress = request.NotificationEvent.CcAddress;
            notifyEvent.DataPacket = request.NotificationEvent.DataPacket;
            notifyEvent.Attachment = request.NotificationEvent.Attachment;
            notifyEvent.FromEmailAddress = request.NotificationEvent.FromEmailAddress;
            notifyEvent.NotificationEventDate = DateTime.UtcNow;
            notifyEvent.NotificationEventTypeId = request.NotificationEvent.NotificationEventTypeId;
            notifyEvent.NotificationStatusId = request.NotificationEvent.NotificationStatusId;
            notifyEvent.NotificationTemplateId = request.NotificationEvent.NotificationTemplateId;
            notifyEvent.Subject = request.NotificationEvent.Subject;
            notifyEvent.ToEmailAddress = request.NotificationEvent.ToEmailAddress;
            notifyEvent.PortalId = request.NotificationEvent.PortalId;
            notifyEvent.UniqueId = request.NotificationEvent.UniqueId;
            if (request.NotificationEvent.UserId > 0)
                notifyEvent.UserId = request.NotificationEvent.UserId;

            if (request.NotificationEvent.Id.HasValue)
            {
                notifyEvent.Id = request.NotificationEvent.Id.Value;
                context.NotificationEvents.Attach(notifyEvent);
                context.Entry(notifyEvent).State = EntityState.Modified;
            }
            else
                context.NotificationEvents.Add(notifyEvent);
            context.SaveChanges();
            AddOrEditNotificationEventResponse response = new AddOrEditNotificationEventResponse();
            response.NotificationEventId = notifyEvent.Id;
            return response;

        }

        public void BulkAddNotificationEvent(BulkAddNotificationEventRequest request)
        {
            //map to DAL and insert

            DAL.NotificationEvent notifyEvent = null;
            using (var scope = new System.Transactions.TransactionScope())
            {
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    //context1.Configuration.AutoDetectChangesEnabled = false;
                    foreach (NotificationEventDto dto in request.NotificationEvents)
                    {
                        notifyEvent = new DAL.NotificationEvent();
                        notifyEvent.BccAddress = dto.BccAddress;
                        notifyEvent.CcAddress = dto.CcAddress;
                        notifyEvent.DataPacket = dto.DataPacket;
                        notifyEvent.FromEmailAddress = dto.FromEmailAddress;
                        notifyEvent.NotificationEventDate = DateTime.UtcNow;
                        notifyEvent.NotificationEventTypeId = dto.NotificationEventTypeId;
                        notifyEvent.NotificationStatusId = dto.NotificationStatusId;
                        notifyEvent.NotificationTemplateId = dto.NotificationTemplateId;
                        notifyEvent.Subject = dto.Subject;
                        notifyEvent.ToEmailAddress = dto.ToEmailAddress;
                        notifyEvent.UserId = dto.UserId;
                        context1.NotificationEvents.Add(notifyEvent);
                    }

                    context1.SaveChanges();
                }

                scope.Complete();

            }

        }

        public GetNotificationEventResponse GetNotificationEvent(GetNotificationEventRequest request)
        {
            var evt = Context.NotificationEvents.Where(x => x.Id == request.NotificationEventId).FirstOrDefault();
            GetNotificationEventResponse response = new GetNotificationEventResponse();
            response.NotificationEvent = Utility.mapper.Map<DAL.NotificationEvent, DTO.NotificationEventDto>(evt);
            return response;
        }

        public GetNotificationEventByUniqueIdResponse GetNotificationEventByUniqueId(GetNotificationEventRequest request)
        {
            var evt = Context.NotificationEvents.Where(x => x.UniqueId == request.UniqueId && x.PortalId == request.PortalId &&
            (!request.NotificationEventTypeId.HasValue || x.NotificationEventTypeId == request.NotificationEventTypeId)).ToList();
            GetNotificationEventByUniqueIdResponse response = new GetNotificationEventByUniqueIdResponse();
            response.NotificationEvent = Utility.mapper.Map<IList<DAL.NotificationEvent>, IList<DTO.NotificationEventDto>>(evt);
            return response;
        }

        public GetNotificationEventByUserIdResponse GetNotificationEventByUserId(GetNotificationEventByUserIdRequest request)
        {
            var evt = Context.NotificationEvents.Where(x => x.UserId == request.UserId && x.NotificationEventTypeId == request.NotificationEventTypeId).ToList();
            GetNotificationEventByUserIdResponse response = new GetNotificationEventByUserIdResponse();
            response.NotificationEvent = Utility.mapper.Map<IList<DAL.NotificationEvent>, IList<DTO.NotificationEventDto>>(evt);
            return response;
        }

        public GetNotificationTemplateResponse GetNotificationTemplate(GetNotificationTemplateRequest request)
        {

            DAL.NotificationTemplate template = null;
            if (request.NotificationTemplateId.HasValue)
            {
                //check in dict first
                GetNotificationTemplateResponse cachedItem;
                if (dictNotificationTemplate.TryGetValue(request.NotificationTemplateId.Value, out cachedItem))
                {
                    return cachedItem;
                }

                template = Context.NotificationTemplates.Include("NotificationTemplateTranslations").Where(x => x.Id == request.NotificationTemplateId.Value).FirstOrDefault();

            }
            else if (request.NotificationEventTypeId.HasValue)
            {
                //since we have only one template now this should work
                template = Context.NotificationTemplates.Where(x => x.NotificationEventTypeId == request.NotificationEventTypeId.Value).FirstOrDefault();

            }
            GetNotificationTemplateResponse response = new GetNotificationTemplateResponse();
            response.NotificationTemplate = Utility.mapper.Map<DAL.NotificationTemplate, DTO.NotificationTemplateDto>(template);
            //add to dictionary
            if (!dictNotificationTemplate.Keys.Contains(response.NotificationTemplate.Id))
            {
                dictNotificationTemplate.Add(response.NotificationTemplate.Id, response);
            }
            return response;
        }

        public CreateNotificationMessageResponse CreateNewNotificationMessage(CreateNotificationMessageRequest request)
        {
            //map to DAL and insert

            DAL.NotificationMessage msg = new DAL.NotificationMessage();
            msg.BccAddress = request.NotificationMessage.BccAddress;
            msg.CcAddress = request.NotificationMessage.CcAddress;
            msg.FromAddress = request.NotificationMessage.FromAddress;
            msg.MessageBody = request.NotificationMessage.MessageBody;
            msg.NotificationEventID = request.NotificationMessage.NotificationEventId;
            msg.SubjectLine = request.NotificationMessage.Subject;
            msg.ToAddress = request.NotificationMessage.ToAddress;
            msg.Attachment = request.NotificationMessage.Attachment;
            msg.Date = DateTime.UtcNow;

            using (var ctx = base.GetContext)
            {
                ctx.NotificationMessages.Add(msg);
                ctx.SaveChanges();
            }

            CreateNotificationMessageResponse response = new CreateNotificationMessageResponse();
            response.NotificationMessageId = msg.Id;
            return response;

        }

        public ListNotificationMessageResponse ListNotificationMessage(ListNotificationMessageRequest request)
        {
            var notificationMessage = Context.NotificationMessages.Where(x => x.NotificationEvent.UserId == request.UserId).OrderByDescending(x => x.Date).ToList();
            ListNotificationMessageResponse response = new ListNotificationMessageResponse();
            response.NotificationMessage = Utility.mapper.Map<IList<DAL.NotificationMessage>, IList<NotificationMessageDto>>(notificationMessage);
            return response;
        }

        public ListNotificationEventResponse ListNotificationEvents(ListNotificationEventRequest request)
        {
            //process 100 events at a time
            var evts = Context.NotificationEvents.Include("User").Where(x => request.NotificationStatusIds.Contains(x.NotificationStatusId)).Take(100);
            ListNotificationEventResponse response = new ListNotificationEventResponse();
            List<NotificationEventDto> dtos = new List<NotificationEventDto>();
            foreach (DAL.NotificationEvent evt in evts)
            {
                DTO.NotificationEventDto dto = Utility.mapper.Map<DAL.NotificationEvent, DTO.NotificationEventDto>(evt);
                dto.NotificationEventType = NotificationEventTypeDto.GetByKey(dto.NotificationEventTypeId);
                dto.NotificationTemplate = GetNotificationTemplate(new GetNotificationTemplateRequest() { NotificationTemplateId = evt.NotificationTemplateId }).NotificationTemplate;
                if (evt.User != null)
                    dto.OrganizationId = evt.User.OrganizationId;//used for url in email templates
                else if (evt.PortalId.HasValue)
                {
                    dto.OrganizationId = Context.Portals.Where(x => x.Id == evt.PortalId.Value).FirstOrDefault().OrganizationId;
                }
                dtos.Add(dto);

            }
            response.NotificationEvents = dtos;
            return response;
        }

        public ListNotificationTemplatesResponse ListNotificationTemplates(ListNotificationTemplatesRequest request)
        {
            var templates = Context.NotificationTemplates.Include("NotificationTemplateTranslations").Where(x => x.TemplateRendererId == request.NotificationTemplateRendererId.Value);

            ListNotificationTemplatesResponse response = new ListNotificationTemplatesResponse();
            List<NotificationTemplateDto> dtos = new List<NotificationTemplateDto>();
            foreach (DAL.NotificationTemplate template in templates)
            {
                DTO.NotificationTemplateDto dto = Utility.mapper.Map<DAL.NotificationTemplate, DTO.NotificationTemplateDto>(template);
                dtos.Add(dto);

            }
            response.NotificationTemplates = dtos;
            return response;
        }

        public ListNotificationEventTypePortalResponse ListNotificationEventTypePortal(GetNotificationEventTypePortalRequest request)
        {
            ListNotificationEventTypePortalResponse response = new ListNotificationEventTypePortalResponse();
            var notificationEventType = Context.NotificationEventTypes.Include("Portals").Where(x => (request.NotificationEventTypeId == 0 || x.Id == request.NotificationEventTypeId) && (request.PortalId == 0 || (x.Portals.Where(y => y.Id == request.PortalId).Count() > 0))).ToList();
            if (notificationEventType.Count() > 0)
            {
                response.NotificationEventTypePortals = Utility.mapper.Map<IList<DAL.NotificationEventType>, IList<NotificationEventTypeDto>>(notificationEventType);
                response.Portals = Utility.mapper.Map<IList<DAL.Portal>, IList<PortalDto>>(notificationEventType.FirstOrDefault().Portals.ToList());
            }
            return response;
        }

        public ListNotificationEventTypeResponse ListNotificationEventType()
        {
            ListNotificationEventTypeResponse response = new ListNotificationEventTypeResponse();
            var types = Context.NotificationEventTypes.Include("NotificationCategory").Where(x => x.IsActive == true).ToList();
            response.emails = Utility.mapper.Map<IList<DAL.NotificationEventType>, IList<NotificationEventTypeDto>>(types);
            return response;
        }

        public IList<NotificationCategoryDto> ListNotificationCategory()
        {
            var types = Context.NotificationCategories.ToList();
            return Utility.mapper.Map<IList<DAL.NotificationCategory>, IList<NotificationCategoryDto>>(types);
        }
    }
}
