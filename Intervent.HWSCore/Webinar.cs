using Intervent.HWS.Model;
using Intervent.Utils;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Intervent.HWS
{
    public static class Webinar
    {
        public static ZoomOAuth GenerateOAuthToken(string zoomClientId, string zoomClientSecret, string zoomOAuthURL, string zoomAccountId)
        {
            var modelResponse = Task.Run(() => OAuthToken(zoomClientId, zoomClientSecret, zoomOAuthURL, zoomAccountId));
            return modelResponse.Result;
        }

        public static CreateWebinarResponse CreateWebinar(CreateWebinarRequest request, string token, string zoomAPIURL, string zoomUserId)
        {
            var modelResponse = Task.Run(() => Create(request, token, zoomAPIURL, zoomUserId));
            return modelResponse.Result;
        }

        public static UpdateWebinarResponse UpdateWebinar(UpdateWebinarRequest request, string token, string webinarId, string zoomAPIURL)
        {
            var modelResponse = Task.Run(() => Update(request, token, webinarId, zoomAPIURL));
            return modelResponse.Result;
        }

        public static RegisterUserForWebinarResponse RegisterUserForWebinar(RegisterUserForWebinarRequest request, string token, string meetingId, string zoomAPIURL)
        {
            var modelResponse = Task.Run(() => RegisterUser(request, token, meetingId, zoomAPIURL));
            return modelResponse.Result;
        }

        public static GetWebinarResponse GetWebinarRegistrants(GetWebinarRequest request)
        {
            var modelResponse = Task.Run(() => GetRegistrants(request));
            return modelResponse.Result;
        }

        public static GetWebinarsListResponse GetWebinarsList(string token, string zoomAPIURL, string zoomUserId)
        {
            var modelResponse = Task.Run(() => ListWebinars(token, zoomAPIURL, zoomUserId));
            return modelResponse.Result;
        }

        public static WebinarResponse GetWebinar(string token, string webinarId, string zoomAPIURL)
        {
            var modelResponse = Task.Run(() => Webinars(token, webinarId, zoomAPIURL));
            return modelResponse.Result;
        }

        private static async Task<ZoomOAuth> OAuthToken(string zoomClientId, string zoomClientSecret, string zoomOAuthURL, string zoomAccountId)
        {
            ZoomOAuth processResponse = new ZoomOAuth();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    var authenticationString = $"{zoomClientId}:{zoomClientSecret}";
                    client.BaseAddress = new Uri(zoomOAuthURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString)));

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "account_credentials"),
                        new KeyValuePair<string, string>("account_id", zoomAccountId)
                    });

                    HttpResponseMessage response = await client.PostAsync("/oauth/token", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<ZoomOAuth>(apiJsonResponse);
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

        private static async Task<CreateWebinarResponse> Create(CreateWebinarRequest request, string token, string zoomAPIURL, string zoomUserId)
        {
            CreateWebinarResponse processResponse = new CreateWebinarResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(zoomAPIURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.PostAsJsonAsync("/v2/users/" + zoomUserId + "/webinars", request);

                    if (response.IsSuccessStatusCode)
                    {
                        processResponse.Status = true;
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<CreateWebinarResponse>(apiJsonResponse);
                    }
                    else
                        processResponse.ErrorMsg = response.ReasonPhrase;
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

        private static async Task<UpdateWebinarResponse> Update(UpdateWebinarRequest request, string token, string webinarId, string zoomAPIURL)
        {
            UpdateWebinarResponse processResponse = new UpdateWebinarResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(zoomAPIURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var update = new HttpRequestMessage(new HttpMethod("PATCH"), "/v2/webinars/" + webinarId)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
                    };
                    var response = await client.SendAsync(update);

                    if (response.IsSuccessStatusCode)
                    {
                        processResponse.Status = true;
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<UpdateWebinarResponse>(apiJsonResponse);
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

        private static async Task<RegisterUserForWebinarResponse> RegisterUser(RegisterUserForWebinarRequest request, string token, string meetingId, string zoomAPIURL)
        {
            RegisterUserForWebinarResponse processResponse = new RegisterUserForWebinarResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(zoomAPIURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.PostAsJsonAsync("/v2/webinars/" + meetingId + "/registrants", request);

                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<RegisterUserForWebinarResponse>(apiJsonResponse);
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

        private static async Task<GetWebinarResponse> GetRegistrants(GetWebinarRequest request)
        {
            GetWebinarResponse processResponse = new GetWebinarResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(request.zoomAPIURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.token);

                    var response = await client.GetAsync("/v2/webinars/" + request.webinarId + "/registrants?status=approved&page_size=200");

                    if (response.IsSuccessStatusCode)
                    {
                        processResponse.Status = true;
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse.registerWebinars = JsonConvert.DeserializeObject<GetWebinarResponse.RegisterWebinarsResponse>(apiJsonResponse);
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

        private static async Task<GetWebinarsListResponse> ListWebinars(string token, string zoomAPIURL, string zoomUserId)
        {
            GetWebinarsListResponse processResponse = new GetWebinarsListResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(zoomAPIURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync("/v2/users/" + zoomUserId + "/webinars");

                    if (response.IsSuccessStatusCode)
                    {
                        processResponse.Status = true;
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse.webinarsList = JsonConvert.DeserializeObject<GetWebinarsListResponse.ListWebinar>(apiJsonResponse);
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

        private static async Task<WebinarResponse> Webinars(string token, string webinarId, string zoomAPIURL)
        {
            WebinarResponse processResponse = new WebinarResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(zoomAPIURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync("/v2/webinars/" + webinarId);

                    if (response.IsSuccessStatusCode)
                    {
                        processResponse.Status = true;
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse.webinar = JsonConvert.DeserializeObject<WebinarResponse.Webinar>(apiJsonResponse);
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
