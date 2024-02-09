using Intervent.Web.DTO;

namespace Intervent.Business.EmailTriggers
{
    public class InCompleteLabTrigger : BaseEmailTrigger, IEmailTrigger
    {

        public override int NotificationTemplateId
        {
            get { return 3; }
        }

        public override int NotificationEventTypeId
        {
            get { return 3; }
        }

        public override EmailTriggerCondition Trigger
        {
            get { return EmailTriggerCondition.IncompleteLab; }
        }
    }
}
