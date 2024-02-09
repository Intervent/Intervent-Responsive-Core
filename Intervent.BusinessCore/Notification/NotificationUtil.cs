using Intervent.Web.DTO;

namespace Intervent.Business.Notification
{
    public static class NotificationUtil
    {
        public static string WebsiteUrl(AppEnvironment env)
        {
            if (env == AppEnvironment.PROD)
                return "https://www.myintervent.com";
            else
                return "https://www.myintervent.com";
        }

        public static string WebsiteUrlToken
        {
            get
            {
                return "{WEBSITE_URL}";
            }
        }
    }
}
