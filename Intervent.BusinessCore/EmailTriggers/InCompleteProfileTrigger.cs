using Intervent.Web.DTO;

namespace Intervent.Business.EmailTriggers
{
    public class InCompleteProfileTrigger : BaseEmailTrigger, IEmailTrigger
    {
        public override int NotificationTemplateId
        {
            get { return 1; }
        }

        public override int NotificationEventTypeId
        {
            get { return 1; }
        }

        public override EmailTriggerCondition Trigger
        {
            get { return EmailTriggerCondition.IncompletProfile; }
        }
    }
}
