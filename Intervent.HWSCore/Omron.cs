using Intervent.HWS.Model;
using Intervent.Utils;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Intervent.HWS
{
    public static class Omron
    {
        public static async Task<OmronOAuth> GetOAuthAsync(FormUrlEncodedContent content, string omronApiUrl)
        {
            OmronOAuth processResponse = new OmronOAuth();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(omronApiUrl);
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    HttpResponseMessage response = await client.PostAsync("connect/token", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<OmronOAuth>(apiJsonResponse);
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

        public static async Task<ProcessResponse> RevokeAccessAsync(FormUrlEncodedContent content, string omronApiUrl)
        {
            ProcessResponse processResponse = new ProcessResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(omronApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.PostAsync("connect/revocation", content);

                    if (response.IsSuccessStatusCode)
                    {
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

        public static async Task<OmronMeasurement> GetMeasurement(FormUrlEncodedContent content, string token, string omronApiUrl)
        {
            OmronMeasurement processResponse = new OmronMeasurement();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(omronApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.PostAsync("api/measurement", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<OmronMeasurement>(apiJsonResponse);
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
    }
}
