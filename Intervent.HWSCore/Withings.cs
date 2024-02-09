using Intervent.HWS.Model;
using Intervent.Utils;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Intervent.HWS
{
    public static class Withings
    {
        public static async Task<WithingsNonce> GetNonceAsync(FormUrlEncodedContent content, string withingsApiUrl)
        {
            WithingsNonce processResponse = new WithingsNonce();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(withingsApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.PostAsync("/v2/signature", content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<WithingsNonce>(responseString);
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

        public static async Task<WithingsOAuth2> GetOAuth2Async(FormUrlEncodedContent content, string withingsApiUrl)
        {
            WithingsOAuth2 processResponse = new WithingsOAuth2();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(withingsApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    var response = await client.PostAsync("/v2/oauth2", content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<WithingsOAuth2>(responseString);
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

        public static async Task<WithingsRevokeUser> RevokeAccessAsync(FormUrlEncodedContent content, string withingsApiUrl)
        {
            WithingsRevokeUser processResponse = new WithingsRevokeUser();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(withingsApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    var response = await client.PostAsync("/v2/oauth2", content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<WithingsRevokeUser>(responseString);
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

        public static async Task<WithingsOAuth2> GetRefreshOauth2Async(FormUrlEncodedContent content, string withingsApiUrl)
        {
            WithingsOAuth2 processResponse = new WithingsOAuth2();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(withingsApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    var response = await client.PostAsync("/v2/oauth2", content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<WithingsOAuth2>(responseString);
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

        public static WithingsSleep GetSleep(FormUrlEncodedContent content, string token, string withingsApiUrl)
        {
            var modelResponse = Task.Run(() => GetSleepAsync(content, token, withingsApiUrl));
            return modelResponse.Result;
        }

        private static async Task<WithingsSleep> GetSleepAsync(FormUrlEncodedContent content, string token, string withingsApiUrl)
        {
            WithingsSleep processResponse = new WithingsSleep();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(withingsApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept-language", "en_US");

                    var response = await client.PostAsync("/v2/sleep", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<WithingsSleep>(apiJsonResponse);
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

        public static WithingsSummary GetSummary(FormUrlEncodedContent content, string token, string withingsApiUrl)
        {
            var modelResponse = Task.Run(() => GetSummaryAsync(content, token, withingsApiUrl));
            return modelResponse.Result;
        }

        private static async Task<WithingsSummary> GetSummaryAsync(FormUrlEncodedContent content, string token, string withingsApiUrl)
        {
            WithingsSummary processResponse = new WithingsSummary();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(withingsApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept-language", "en_US");

                    var response = await client.PostAsync("/v2/measure", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<WithingsSummary>(apiJsonResponse);
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

        public static WithingsWorkouts GetWorkouts(FormUrlEncodedContent content, string token, string withingsApiUrl)
        {
            var modelResponse = Task.Run(() => GetWorkoutsAsync(content, token, withingsApiUrl));
            return modelResponse.Result;
        }

        private static async Task<WithingsWorkouts> GetWorkoutsAsync(FormUrlEncodedContent content, string token, string withingsApiUrl)
        {
            WithingsWorkouts processResponse = new WithingsWorkouts();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(withingsApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept-language", "en_US");

                    var response = await client.PostAsync("/v2/measure", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<WithingsWorkouts>(apiJsonResponse);
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

        public static WithingsMeasurements GetMeasurementsLog(FormUrlEncodedContent content, string token, string withingsApiUrl)
        {
            var modelResponse = Task.Run(() => GetMeasurementsLogAsync(content, token, withingsApiUrl));
            return modelResponse.Result;
        }

        private static async Task<WithingsMeasurements> GetMeasurementsLogAsync(FormUrlEncodedContent content, string token, string withingsApiUrl)
        {
            WithingsMeasurements processResponse = new WithingsMeasurements();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(withingsApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept-language", "en_US");

                    var response = await client.PostAsync("/measure", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<WithingsMeasurements>(apiJsonResponse);
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
