using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace Intervent.HWS.Request
{
    public class LivongoRootobjectRequest
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public Entry[] entry { get; set; }

        public LivongoRootobjectRequest()
        {
            type = "message";
            resourceType = "Bundle";
            id = string.Format("urn:uuid:{0}", Guid.NewGuid().ToString());
            entry = new Entry[2];
            entry[0] = new Entry(true);
            entry[1] = new Entry(false);
            entry[1].fullUrl = (entry[0].resource as HeaderResource).data[0].reference;
        }
    }

    public class Entry
    {
        public string fullUrl { get; set; }
        public Resource resource { get; set; }

        public Entry(bool resourceHeader)
        {
            fullUrl = string.Format("urn:uuid:{0}", Guid.NewGuid().ToString());
            if (resourceHeader)
                resource = new HeaderResource();
            else
                resource = new ParameterResource();
        }
    }

    public class ParameterResource : Resource
    {
        public Parameter[] parameter { get; set; }

        public ParameterResource()
        {
            resourceType = "Parameters";
            parameter = new Parameter[4];
            parameter[0] = new StringParameter();
            parameter[1] = new BooleanParameter();
            parameter[2] = new IntergerParameter();
            parameter[3] = new Integer2Parameter();
        }
    }

    public class HeaderResource : Resource
    {
        public string timestamp { get; set; }

        [JsonProperty("event")]
        public Event _event { get; set; }
        public Datum[] data { get; set; }


        public HeaderResource()
        {
            resourceType = "MessageHeader";
            timestamp = DateTime.Now.ToString("o");
            _event = new Event();
            data = new Datum[1];
            data[0] = new Datum();
            data[0].reference = string.Format("urn:uuid:{0}", Guid.NewGuid().ToString());
        }
    }

    public class Resource
    {
        public string resourceType { get; set; }
    }

    public class Event
    {
        public string system { get; set; }
        public string code { get; set; }

        public Event()
        {
            system = "http://hl7.org/fhir/restful-interaction";
            code = "search-type";
        }
    }

    public class Datum
    {
        public string reference { get; set; }
    }

    public class IntergerParameter : Parameter
    {
        public int valueInteger { get; set; }

        public IntergerParameter()
        {
            name = "pendingTimeout";
            valueInteger = 120;
        }
    }

    public class BooleanParameter : Parameter
    {
        public bool? valueBoolean { get; set; }

        public BooleanParameter()
        {
            name = "unread";
            valueBoolean = true;
        }
    }

    public class Integer2Parameter : Parameter
    {
        public int valueInteger { get; set; }

        public Integer2Parameter()
        {
            name = "_count";
            valueInteger = 120;
        }
    }

    public class StringParameter : Parameter
    {
        public string valueString { get; set; }

        public StringParameter()
        {
            name = "resourceType";
            valueString = "Observation";
        }
    }

    public class Parameter
    {
        public string name { get; set; }

    }

    public class AckRootobject
    {
        public string resourceType { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public AckEntry[] entry { get; set; }

        public AckRootobject(List<string> ackList, LivongoRootobjectRequest resource)
        {
            resourceType = "Bundle";
            type = "message";
            id = string.Format("urn:uuid:{0}", Guid.NewGuid().ToString());
            entry = new AckEntry[ackList.Count + 1];
            entry[0] = new ReferenceEntry(resource);
            int index = 1;
            foreach (var reference in ackList)
            {
                entry[index++] = new ContentEntry(reference);
            }
        }
    }

    public class AckEntry
    {


    }

    public class ReferenceEntry : AckEntry
    {
        public string fullUrl { get; set; }
        public HeaderResource resource { get; set; }

        public ReferenceEntry(LivongoRootobjectRequest request)
        {
            fullUrl = request.entry[0].fullUrl;
            resource = request.entry[0].resource as HeaderResource;
            resource.timestamp = DateTime.Now.ToString("o");
        }
    }

    public class ContentEntry : AckEntry
    {
        public AckResource resource { get; set; }

        public ContentEntry(string reference)
        {
            resource = new AckResource(reference);
        }
    }

    public class AckResource
    {
        public string resourceType { get; set; }
        public AckRequest request { get; set; }
        public AckOutcome outcome { get; set; }

        public AckResource(string reference)
        {
            resourceType = "ProcessResponse";
            request = new AckRequest(reference);
            outcome = new AckOutcome();
        }
    }

    public class AckRequest
    {
        public string reference { get; set; }

        public AckRequest(string ackReference)
        {
            reference = ackReference;
        }
    }

    public class AckOutcome
    {
        public string code { get; set; }

        public AckOutcome()
        {
            code = "AA";
        }
    }


}
