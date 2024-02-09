using Intervent.HWS.Model;
using Intervent.Utils;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Intervent.HWS
{
    public static class Garmin
    {
        public static async Task<GarminOAuth1> GetRequestToken(string requestTokenUrl, StringBuilder sb, string garminApiUrl, string garminConnectUrl)
        {
            GarminOAuth1 processResponse = new GarminOAuth1();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(garminApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsync(requestTokenUrl + "?" + sb.ToString(), null);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        string responseString = await response.Content.ReadAsStringAsync();
                        //var queryString = HttpUtility.ParseQueryString(responseString);
                        //processResponse.oauth_token = queryString["oauth_token"];
                        //processResponse.oauth_token_secret = queryString["oauth_token_secret"];
                        processResponse.url = garminConnectUrl + "oauthConfirm?oauth_token=" + processResponse.oauth_token;
                    }
                }
            }
            catch (Exception ex)
            {
                processResponse.Exception = ex;
                processResponse.ErrorMsg = ex.Message;
            }
            return processResponse;
        }

        public static async Task<GarminOAuth1> GetAccessToken(string accessTokenUrl, StringBuilder sb, string garminApiUrl)
        {
            GarminOAuth1 processResponse = new GarminOAuth1();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(garminApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsync(accessTokenUrl + "?" + sb.ToString(), null);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        //var queryString = HttpUtility.ParseQueryString(responseString);
                        //processResponse.oauth_token = queryString["oauth_token"];
                        //processResponse.oauth_token_secret = queryString["oauth_token_secret"];
                        processResponse.Status = true;
                    }
                    processResponse.StatusCode = response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                processResponse.Exception = ex;
                processResponse.ErrorMsg = ex.Message;
            }
            return processResponse;
        }

        public static async Task<GarminOAuth1> GetUserDetails(string getUserUrl, StringBuilder oAuthHeader, string garminUserEndPointUrl)
        {
            GarminOAuth1 processResponse = new GarminOAuth1();
            try
            {

                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(garminUserEndPointUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //var oauthHeaders = HttpUtility.ParseQueryString(oAuthHeader.ToString());
                    Regex reg = new Regex(@"%[a-f0-9]{2}");
                    //var oauth_signature = reg.Replace(HttpUtility.UrlEncode(oauthHeaders["oauth_signature"]), m => m.Value.ToUpperInvariant());
                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", string.Format("oauth_consumer_key =\"{0}\",oauth_token=\"{1}\",oauth_signature_method=\"{2}\",oauth_timestamp=\"{3}\",oauth_nonce=\"{4}\",oauth_version=\"{5}\",oauth_signature=\"{6}\"", oauthHeaders["oauth_consumer_key"], oauthHeaders["oauth_token"], oauthHeaders["oauth_signature_method"], oauthHeaders["oauth_timestamp"], oauthHeaders["oauth_nonce"], oauthHeaders["oauth_version"], oauth_signature));

                    var response = await client.GetAsync(getUserUrl);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;
                        var garminResponse = JsonConvert.DeserializeObject<GarminResponse>(responseString);
                        processResponse.userId = garminResponse.userId;
                    }
                }
            }
            catch (Exception ex)
            {
                processResponse.Exception = ex;
                processResponse.ErrorMsg = ex.Message;
            }
            return processResponse;
        }

        public static async Task<GarminOAuth1> UserDeRegistration(string getUserUrl, StringBuilder sb, string garminUserEndPointUrl)
        {
            GarminOAuth1 processResponse = new GarminOAuth1();
            try
            {

                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(garminUserEndPointUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // var oauthHeaders = HttpUtility.ParseQueryString(sb.ToString());
                    Regex reg = new Regex(@"%[a-f0-9]{2}");
                    //var oauth_signature = reg.Replace(HttpUtility.UrlEncode(oauthHeaders["oauth_signature"]), m => m.Value.ToUpperInvariant());
                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", string.Format("oauth_consumer_key =\"{0}\",oauth_token=\"{1}\",oauth_signature_method=\"{2}\",oauth_timestamp=\"{3}\",oauth_nonce=\"{4}\",oauth_version=\"{5}\",oauth_signature=\"{6}\"", oauthHeaders["oauth_consumer_key"], oauthHeaders["oauth_token"], oauthHeaders["oauth_signature_method"], oauthHeaders["oauth_timestamp"], oauthHeaders["oauth_nonce"], oauthHeaders["oauth_version"], oauth_signature));

                    var response = await client.DeleteAsync(getUserUrl);
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        processResponse.Status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                processResponse.Exception = ex;
                processResponse.ErrorMsg = ex.Message;
            }
            return processResponse;
        }

    }
}
