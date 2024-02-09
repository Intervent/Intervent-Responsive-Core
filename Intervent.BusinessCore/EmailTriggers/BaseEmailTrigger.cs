using Intervent.Business.Account;
using Intervent.Business.Notification;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace Intervent.Business.EmailTriggers
{
    public abstract class BaseEmailTrigger
    {
        protected INotificationManager _notificationManager = null;
        protected IAccountManager _accountManager = null;
        protected ParticipantReader _participantReader = null;
        protected EmailTriggerReader _emailTriggerReader = null;
        protected AdminReader _adminReader = null;
        protected AccountReader _accountReader = null;
        public BaseEmailTrigger()
        {
            _notificationManager = new NotificationManager();
            _accountManager = new AccountManager();
            _participantReader = new ParticipantReader();
            _emailTriggerReader = new EmailTriggerReader();
            _adminReader = new AdminReader();
        }

        public abstract int NotificationEventTypeId { get; }

        public abstract int NotificationTemplateId { get; }

        public abstract EmailTriggerCondition Trigger { get; }

        public virtual IEnumerable<EmailTriggerDto> ProcessTrigger()
        {
            List<EmailTriggerDto> events = new List<EmailTriggerDto>();
            var users = _emailTriggerReader.ListUsers(new ListUsersEmailTriggerRequest() { NotificationEventTypeId = NotificationEventTypeId, TriggerCondition = Trigger }).Users;

            foreach (UserDto user in users)
            {
                if (user != null)
                {

                    EmailTriggerDto dto = new EmailTriggerDto();
                    NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                    xsltDto.UserFirstName = user.FirstName;
                    xsltDto.UserLastName = user.LastName;
                    dto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                    dto.TemplateId = NotificationTemplateId;
                    dto.UserId = user.Id;
                    events.Add(dto);
                }
            }

            return events;
        }

        protected string GetCoachName(int userId)
        {
            var user = _accountReader.FindUsers(new FindUsersRequest() { UserId = userId }).Users;
            return (user != null && user.Count() > 0) ? user.First().FirstName + " " + user.First().LastName : "";
        }
    }
}
