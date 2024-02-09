using Intervent.Web.DTO;

namespace Intervent.Business.EmailTriggers
{
    public class FollowUpTrigger : BaseEmailTrigger, IEmailTrigger
    {

        public override int NotificationTemplateId
        {
            get { return 10; }
        }

        public override int NotificationEventTypeId
        {
            get { return 10; }
        }

        public override EmailTriggerCondition Trigger
        {
            get { return EmailTriggerCondition.FollowUp; }
        }
    }
}
