using System.Text;
using System.Xml.Serialization;

namespace Intervent.Web.DTO
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }

    public class DTOUtil
    {
        public static string SerializeObjectToXml<T>(object instance)
        {
            using (StringWriter sw = new Utf8StringWriter())
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(sw, instance);
                return sw.ToString();
            }
        }

        public static T SerializeXMLToObject<T>(string s)
        {
            using (StringReader sr = new StringReader(s))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                return (T)xs.Deserialize(sr);
            }
        }
    }
}
