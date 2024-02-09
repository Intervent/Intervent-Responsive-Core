namespace Intervent.HWS
{
    public class LivongoRootobject
    {
        public string resourceType { get; set; }
        public string type { get; set; }
        public Entry[] entry { get; set; }
    }

    public class Entry
    {
        public string fullUrl { get; set; }
        public Resource resource { get; set; }
    }

    public class Resource
    {
        public string resourceType { get; set; }
        public DateTime timestamp { get; set; }
        public Event _event { get; set; }
        public Response response { get; set; }
        public Datum[] data { get; set; }
        public string id { get; set; }
        public Contained[] contained { get; set; }
        public Extension[] extension { get; set; }
        public string status { get; set; }
        public Code code { get; set; }
        public Subject subject { get; set; }
        public DateTime effectiveDateTime { get; set; }
        public Valuequantity valueQuantity { get; set; }
    }

    public class Event
    {
        public string system { get; set; }
        public string code { get; set; }
    }

    public class Response
    {
        public string identifier { get; set; }
        public string code { get; set; }
    }

    public class Code
    {
        public Coding[] coding { get; set; }
    }

    public class Coding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class Subject
    {
        public string reference { get; set; }
    }

    public class Valuequantity
    {
        public int value { get; set; }
        public string unit { get; set; }
        public string system { get; set; }
        public string code { get; set; }
    }

    public class Datum
    {
        public string reference { get; set; }
    }

    public class Contained
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public Identifier[] identifier { get; set; }
    }

    public class Identifier
    {
        public string system { get; set; }
        public string value { get; set; }
    }

    public class Extension
    {
        public string url { get; set; }
        public string valueString { get; set; }
        public string valueUri { get; set; }
    }

}
