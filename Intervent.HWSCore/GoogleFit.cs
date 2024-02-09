using Intervent.HWS.Model;
using Intervent.Utils;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Intervent.HWS
{
    public static class GoogleFit
    {
        private static readonly string GoogleFitAuthUrl = "https://oauth2.googleapis.com/";

        private static readonly string GoogleFitApiUrl = "https://www.googleapis.com/";

        public static async Task<GoogleFitOAuth> GetOAuthAsync(FormUrlEncodedContent content)
        {
            GoogleFitOAuth processResponse = new GoogleFitOAuth();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(GoogleFitAuthUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.PostAsync("/token", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<GoogleFitOAuth>(apiJsonResponse);
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

        public static async Task<ProcessResponse> RevokeAccessAsync(FormUrlEncodedContent content)
        {
            ProcessResponse processResponse = new ProcessResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(GoogleFitAuthUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.PostAsync("/revoke", content);

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

        public static GoogleFitProfile GetProfileDetails(string token)
        {
            var modelResponse = Task.Run(() => GetProfileDetailsAsync(token));
            return modelResponse.Result;
        }

        private static async Task<GoogleFitProfile> GetProfileDetailsAsync(string token)
        {
            GoogleFitProfile processResponse = new GoogleFitProfile();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(GoogleFitApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync("/userinfo/v2/me");

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<GoogleFitProfile>(apiJsonResponse);
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

        public static GoogleFitness GetFitnessDetails(string token, object content)
        {
            var modelResponse = Task.Run(() => GetFitnessDetailsAsync(token, content));
            return modelResponse.Result;
        }

        private static async Task<GoogleFitness> GetFitnessDetailsAsync(string token, object content)
        {
            GoogleFitness processResponse = new GoogleFitness();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(GoogleFitApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.PostAsJsonAsync("/fitness/v1/users/me/dataset:aggregate", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<GoogleFitness>(apiJsonResponse);
                        processResponse.Status = true;
                    }
                    else if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse.error = JsonConvert.DeserializeObject<GoogleError>(apiJsonResponse);
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
