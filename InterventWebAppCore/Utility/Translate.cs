using Intervent.Web.DataLayer;

namespace InterventWebApp
{
    public static class Translate
    {
        public static IHttpContextAccessor _httpContextAccessor;

        private static List<string> SupportedLanguages = new List<string>() { "es", "en-us", "ar", "ar-ae", "fr", "pt" };

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string GetSessionValue(string key)
        {
            return _httpContextAccessor?.HttpContext?.Session.GetString(key);
        }

        public static string GetLanguage()
        {
            try
            {
                string languageCode = string.Empty;
                if (!string.IsNullOrEmpty(GetSessionValue(SessionContext.LanguagePreference)))
                {
                    languageCode = GetSessionValue(SessionContext.LanguagePreference);
                } 
                else if (!string.IsNullOrEmpty(_httpContextAccessor?.HttpContext?.Request.Headers["Accept-Language"]))
                {
                    string acceptLanguageHeader = _httpContextAccessor?.HttpContext?.Request.Headers["Accept-Language"];
                    string[] userLanguages = acceptLanguageHeader?.Split(',');
                    languageCode = userLanguages[0];
                }
                if (!string.IsNullOrEmpty(languageCode) && SupportedLanguages.Contains(languageCode.ToLower()))
                    return languageCode;
            }
            catch { }
            return "en-US";
        }

        public static string GetCurrentLanguage()
        {
            return GetLanguage().ToLower();
        }

        public static string GetCurrentLanguageName()
        {
            switch (GetLanguage().ToLower())
            {
                case "en-us":
                    return Message("L1921");

                case "es":
                    return Message("L1922");

                case "ar":
                    return Message("L3140");

                case "ar-ae":
                    return Message("L3140");

                case "pt":
                    return Message("L4568");

                default:
                    return Message("L1921");
            }
        }

        public static string Message(string key)
        {
            return GlobalTranslator.Message(key, GetLanguage());
        }
    }
}