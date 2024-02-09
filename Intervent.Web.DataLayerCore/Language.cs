using System.Reflection;
using System.Resources;

namespace Intervent.Web.DataLayer
{
    public static class Language
    {
        public static List<string> GetSupportedLanguages()
        {
            string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            List<string> languages = new List<string>();

            for (int i = 0; i < names.Length; i++)
            {
                if (Path.GetExtension(names[i]).Equals(".resources", StringComparison.OrdinalIgnoreCase))
                {
                    languages.Add(Path.GetFileNameWithoutExtension(names[i]));
                }
            }

            return languages;
        }

        public static ResourceManager GetResourceManager(string languageCode)
        {
            foreach (string name in GetSupportedLanguages())
            {
                string[] arrLanguageCode = name.Split('.');

                string supportedLanuageCode = arrLanguageCode[arrLanguageCode.Length - 1];

                if (supportedLanuageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase))
                {
                    return new ResourceManager(name, Assembly.GetExecutingAssembly());
                }
            }
            return null;
        }
    }
}