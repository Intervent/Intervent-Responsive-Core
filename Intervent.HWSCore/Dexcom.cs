using Intervent.HWS.Model;
using Intervent.Utils;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Intervent.HWS
{
    public static class Dexcom
    {
        public static async Task<DexcomOAuth2> GetOAuth2Async(string DexcomApiUrl, FormUrlEncodedContent content)
        {
            DexcomOAuth2 processResponse = new DexcomOAuth2();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(DexcomApiUrl);
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    HttpResponseMessage response = await client.PostAsync("/v2/oauth2/token", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<DexcomOAuth2>(apiJsonResponse);
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

        public static DexcomDevices GetDevices(string DexcomApiUrl, string token)
        {
            var modelResponse = Task.Run(() => GetDevicesAsync(DexcomApiUrl, token));
            return modelResponse.Result;
        }

        private static async Task<DexcomDevices> GetDevicesAsync(string DexcomApiUrl, string token)
        {
            DexcomDevices processResponse = new DexcomDevices();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(DexcomApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync("/v3/users/self/devices");

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<DexcomDevices>(apiJsonResponse);
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

        public static DexcomGlucose GetEgvs(string DexcomApiUrl, string token, string startTime, string endTime)
        {
            var modelResponse = Task.Run(() => GetEgvsAsync(DexcomApiUrl, token, startTime, endTime));
            return modelResponse.Result;
        }

        private static async Task<DexcomGlucose> GetEgvsAsync(string DexcomApiUrl, string token, string startTime, string endTime)
        {
            DexcomGlucose processResponse = new DexcomGlucose();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(DexcomApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync("/v3/users/self/egvs?startDate=" + startTime + "&endDate=" + endTime);

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<DexcomGlucose>(apiJsonResponse);
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
