using System.Resources;
using System.Text.RegularExpressions;

namespace Intervent.Web.DataLayer
{
    public static class GlobalTranslator
    {
        public static string Message(string key, string language)
        {
            ResourceManager resMan = Language.GetResourceManager(language);
            if (resMan == null)
                return key;

            string originalKey = key;

            key = Regex.Replace(key, "[ ./]", "_");

            try
            {
                string value = resMan.GetString(key);
                if (value != null)
                    return value;
                return originalKey;
            }
            catch (MissingManifestResourceException)
            {
                return originalKey;
            }
            catch (NullReferenceException)
            {
                return originalKey;
            }
        }
    }
}
