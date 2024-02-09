using Intervent.HWS.Model;
using Intervent.Utils;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Intervent.HWS
{
    public static class Fitbit
    {
        public static async Task<FitbitOAuth2> GetOAuth2Async(FormUrlEncodedContent content, string clientIdConcatSecret, string fitbitApiUrl)
        {
            FitbitOAuth2 processResponse = new FitbitOAuth2();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(fitbitApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", clientIdConcatSecret);

                    var response = await client.PostAsync("/oauth2/token", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<FitbitOAuth2>(apiJsonResponse);
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

        public static async Task<ProcessResponse> RevokeAccessAsync(FormUrlEncodedContent content, string clientIdConcatSecret, string fitbitApiUrl)
        {
            ProcessResponse processResponse = new ProcessResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(fitbitApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", clientIdConcatSecret);

                    var response = await client.PostAsync("/oauth2/revoke", content);

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

        public static FitbitWeight GetWeight(string token, string userId, string fitbitApiUrl)
        {
            var modelResponse = Task.Run(() => GetWeightAsync(token, userId, fitbitApiUrl));
            return modelResponse.Result;
        }

        private static async Task<FitbitWeight> GetWeightAsync(string token, string userId, string fitbitApiUrl)
        {
            FitbitWeight processResponse = new FitbitWeight();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(fitbitApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept-language", "en_US");

                    var response = await client.GetAsync("/1/user/" + userId + "/body/log/weight/date/" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "/1m.json");

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<FitbitWeight>(apiJsonResponse);
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

        public static FitbitSleep GetSleep(string token, string userId, DateTime date, string fitbitApiUrl)
        {
            var modelResponse = Task.Run(() => GetSleepAsync(token, userId, date, fitbitApiUrl));
            return modelResponse.Result;
        }

        private static async Task<FitbitSleep> GetSleepAsync(string token, string userId, DateTime date, string fitbitApiUrl)
        {
            FitbitSleep processResponse = new FitbitSleep();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(fitbitApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept-language", "en_US");

                    var response = await client.GetAsync("/1.2/user/" + userId + "/sleep/date/" + date.ToString("yyyy-MM-dd") + ".json");

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<FitbitSleep>(apiJsonResponse);
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

        public static FitbitFood GetFood(string token, string userId, DateTime date, string fitbitApiUrl)
        {
            var modelResponse = Task.Run(() => GetFoodAsync(token, userId, date, fitbitApiUrl));
            return modelResponse.Result;
        }

        private static async Task<FitbitFood> GetFoodAsync(string token, string userId, DateTime date, string fitbitApiUrl)
        {
            FitbitFood processResponse = new FitbitFood();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(fitbitApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept-language", "en_US");

                    var response = await client.GetAsync("/1/user/" + userId + "/foods/log/date/" + date.ToString("yyyy-MM-dd") + ".json");

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<FitbitFood>(apiJsonResponse);
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

        public static FitbitActivitySummary GetActivitySummary(string token, string userId, DateTime date, string fitbitApiUrl)
        {
            var modelResponse = Task.Run(() => GetActivitySummaryAsync(token, userId, date, fitbitApiUrl));
            return modelResponse.Result;
        }

        public static FitbitProfile GetProfileDetails(string token, string userId, string fitbitApiUrl)
        {
            var modelResponse = Task.Run(() => GetProfileDetailsAsync(token, userId, fitbitApiUrl));
            return modelResponse.Result;
        }

        public static FitbitScope GetUsersScope(FormUrlEncodedContent content, string token, string fitbitApiUrl)
        {
            var modelResponse = Task.Run(() => GeUsersScopeAsync(content, token, fitbitApiUrl));
            return modelResponse.Result;
        }

        private static async Task<FitbitScope> GeUsersScopeAsync(FormUrlEncodedContent content, string token, string fitbitApiUrl)
        {
            FitbitScope processResponse = new FitbitScope();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(fitbitApiUrl);
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept-language", "en_US");

                    var response = await client.PostAsync("/1.1/oauth2/introspect", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<FitbitScope>(apiJsonResponse);
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

        private static async Task<FitbitProfile> GetProfileDetailsAsync(string token, string userId, string fitbitApiUrl)
        {
            FitbitProfile processResponse = new FitbitProfile();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(fitbitApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept-language", "en_US");

                    var response = await client.GetAsync("/1/user/" + userId + "/profile.json");

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<FitbitProfile>(apiJsonResponse);
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

        private static async Task<FitbitActivitySummary> GetActivitySummaryAsync(string token, string userId, DateTime date, string fitbitApiUrl)
        {
            FitbitActivitySummary processResponse = new FitbitActivitySummary();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(fitbitApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("accept-language", "en_US");

                    var response = await client.GetAsync("/1/user/" + userId + "/activities/date/" + date.ToString("yyyy-MM-dd") + ".json");

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<FitbitActivitySummary>(apiJsonResponse);
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
