using Intervent.Web.DTO;

namespace Intervent.Business.EmailTriggers
{
    public class IncompleteHRATrigger : BaseEmailTrigger, IEmailTrigger
    {
        public override int NotificationTemplateId
        {
            get { return 2; }
        }

        public override int NotificationEventTypeId
        {
            get { return 2; }
        }

        public override EmailTriggerCondition Trigger
        {
            get { return EmailTriggerCondition.IncompleteHRA; }
        }


    }
}
