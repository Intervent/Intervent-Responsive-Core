using Newtonsoft.Json;
using System.Net;
using System.Runtime.Serialization;

namespace Intervent.HWS
{
    //When we are not able to process the request return error message and log it or email it
    //TODO: ignore in json response 
    public class ProcessResponse
    {
        [JsonIgnore]
        public bool Status { get; set; }
        [IgnoreDataMember]
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
        [IgnoreDataMember]
        [JsonIgnore]
        public string Request { get; set; }
        [IgnoreDataMember]
        [JsonIgnore]
        public Exception Exception { get; set; }
        [IgnoreDataMember]
        [JsonIgnore]
        public string ErrorMsg { get; set; }
    }
}
