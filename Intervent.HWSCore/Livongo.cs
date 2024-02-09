using Intervent.HWS.Request;
using Intervent.Utils;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Intervent.HWS
{
    public class LivongoPullResponse
    {
        public LivongoRootobjectRequest Request { get; set; }

        public LivongoRootobject Response { get; set; }
    }

    public class Livongo
    {
        private const string LivongoObservationURL = "https://{0}/v1/fhir/Message/Request/Observation/Organization/INTERVENT";
        private const string LivongoAckURL = "https://{0}/v1/fhir/Message/Acknowledgement/Observation/Organization/INTERVENT";
        ///private const string AuthorizationKey = "Bearer 2-v0R3Sm3uSh9XEiEENmVEFQbSDyfv2BozLaYA2qvzeKQ2HJjZxID2BPVHOhRonvuwV4C3/pv3xw9yQdv7c7ajwa+Sm0FLZgKrf724A3SAFqIGtVvuh0CsbRIgjXVUYIQ9K9EkMYe3tOMYP1NlVgWpFuWNfHJyzZelChe95R5kRiAA3PTZvXyAVGfdrGG7K01k2OoT2FtoV21yIKKEV1TODe0Tz37vEr2WdDXvpHVjaMIm/VC3/OOS97TltNwdILSObLsgFcX6tbyV5k+Ah/Ndm18pHd3kHlRAMQlRKo7rK+LhkD5XPzcCaQ2tFz3PBndfr2WIL8w+CUEKn+4540tl3SJPVSaOmYtOZEka9L1foVOSXJDGxRVVa7kF";


        public static async Task<LivongoPullResponse> PullData(string livongoAuthorizationKey, string livongoURL)
        {
            LivongoPullResponse pullResponse = new LivongoPullResponse();

            using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
            {

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", livongoAuthorizationKey);
                // New code:                    
                pullResponse.Request = new LivongoRootobjectRequest();
                var url = string.Format(LivongoObservationURL, livongoURL);
                HttpResponseMessage response = await client.PostAsJsonAsync(url, pullResponse.Request);
                if (response.IsSuccessStatusCode)
                {
                    pullResponse.Response = await response.Content.ReadAsAsync<LivongoRootobject>();

                    return pullResponse;
                }
            }

            return null;
        }

        public static async Task<bool> AckData(LivongoRootobjectRequest request, List<string> references, string livongoAuthorizationKey, string livongoURL)
        {
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", livongoAuthorizationKey);
                    // New code:       
                    var url = string.Format(LivongoAckURL, livongoURL);
                    HttpResponseMessage response = await client.PostAsJsonAsync(url, new AckRootobject(references, request));
                    if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return true;

                    }
                }
            }
            catch { }
            return false;
        }

    }
}