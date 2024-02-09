using Intervent.Web.DTO;
using System.Xml;
using System.Xml.Xsl;

namespace Intervent.Business.Notification
{
    public class XsltRenderer : ITemplateRenderer
    {
        Dictionary<int, XslCompiledTransform> xsltCompiledTransforms;
        public XsltRenderer(string languagePreference)
        {
            xsltCompiledTransforms = new Dictionary<int, XslCompiledTransform>();
            var templates = new NotificationManager().ListXsltRendererNotificationTemplates().NotificationTemplates;
            foreach (var template in templates)
            {
                var transform = new XslCompiledTransform();
                string templateSource = "";
                if (!string.IsNullOrEmpty(languagePreference) && template.NotificationTemplateTranslations.Where(x => x.LanguageCode == languagePreference).FirstOrDefault() != null)
                {
                    templateSource = template.NotificationTemplateTranslations.Where(x => x.LanguageCode == languagePreference).FirstOrDefault().TemplateSource;
                }
                else
                {
                    templateSource = template.TemplateSource;
                }
                using (StringReader sr = new StringReader(templateSource))
                {
                    using (XmlReader xr = XmlReader.Create(sr))
                    {
                        transform.Load(xr);
                    }
                }
                xsltCompiledTransforms.Add(template.Id, transform);
            }


        }

        public string MessageBody(NotificationEventDto notificationEvent)
        {

            // var templateSource = notificationEvent.NotificationTemplate.TemplateSource;
            var xmlDataPacket = notificationEvent.DataPacket;
            var transform = xsltCompiledTransforms[notificationEvent.NotificationTemplateId];

            using (StringReader sr = new StringReader(xmlDataPacket))
            {
                using (XmlReader xr = XmlReader.Create(sr))
                {
                    using (StringWriter sw = new StringWriter())
                    {
                        transform.Transform(xr, null, sw);
                        return sw.ToString();
                    }
                }
            }
        }
    }
}
