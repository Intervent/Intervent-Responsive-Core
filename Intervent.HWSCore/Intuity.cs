namespace Intervent.HWS
{
    using Intervent.Utils;
    using Newtonsoft.Json;
    using System;
    using System.Configuration;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using static Intervent.HWS.IntuityEligibilityLogAPIRequest;
    using static Intervent.HWS.IntuityEligibilityLogAPIResponse;
    using static Intervent.HWS.SendPatternDetailsRequest;

    public static class Intuity
    {
        private static readonly string AuthToken;

        private static readonly string IntuityURL;

        private static readonly string IntuityAPIURL;

        static Intuity()
        {
            IntuityURL = ConfigurationManager.AppSettings["IntuityUrl"];
            AuthToken = ConfigurationManager.AppSettings["IntuityAPIKey"];
            IntuityAPIURL = ConfigurationManager.AppSettings["IntuityAPIURL"];
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        }

        //It should be automatically pooled from Kafka
        public static ProcessResponse PostEligibiltyResponse(EligibilityResponse request)
        {
            var modelResponse = Task.Run(() => PostEligibility(request));
            return modelResponse.Result;
        }

        public static ProcessResponse PostIntuityResponse(object request, string patient_unique_id, string intuityURL, string intuityToken)
        {
            var modelResponse = Task.Run(() => PostIntuityEligibility(request, patient_unique_id, intuityURL, intuityToken));
            return modelResponse.Result;
        }

        public static IntuityEligibilityLogAPIResponse GetIntuityResponse(string patient_unique_id, string intuityURL, string intuityToken)
        {
            var modelResponse = Task.Run(() => GetIntuityEligibility(patient_unique_id, intuityURL, intuityToken));
            return modelResponse.Result;
        }

        public static IntuityEnrolmentURLResponse GetIntuityEnrolmentLink(string patient_unique_id, bool deliver_email, string intuityURL, string intuityToken)
        {
            var modelResponse = Task.Run(() => GetIntuityEnrolmentURL(patient_unique_id, deliver_email, intuityURL, intuityToken));
            return modelResponse.Result;
        }

        public static SendPatternDetailsResponse SendPatternsDetailsRequest(object request, string intuityURL, string intuityToken)
        {
            var modelResponse = Task.Run(() => SendPatternsDetails(request, intuityURL, intuityToken));
            return modelResponse.Result;
        }

        public static ProcessResponse PostVerifyUserResponse(VerifyIntuityUserRequest request)
        {
            var modelResponse = Task.Run(() => PostVerifyUser(request));
            return modelResponse.Result;
        }

        public static IntuityRefreshTokenResponse RefreshIntuityTokenRequest(string refreshToken)
        {
            var modelResponse = Task.Run(() => RefreshIntuityToken(refreshToken));
            return modelResponse.Result;
        }

        public static ProcessResponse SendReminderNotification(NotificationRequest request)
        {
            var modelResponse = Task.Run(() => PostReminder(request));
            return modelResponse.Result;
        }

        private static async Task<ProcessResponse> PostReminder(NotificationRequest request)
        {
            ProcessResponse processResponse = new ProcessResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(request.intuityURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", request.authToken);

                    // New code:
                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/users/notifications/outreach", request);
                    if (response.IsSuccessStatusCode)
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

        private static async Task<ProcessResponse> PostEligibility(EligibilityResponse request)
        {
            ProcessResponse processResponse = new ProcessResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(IntuityURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", AuthToken);

                    // New code:
                    HttpResponseMessage response = await client.PutAsJsonAsync("/api/v1/users", request);
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

        private static async Task<ProcessResponse> PostIntuityEligibility(object request, string patient_unique_id, string intuityURL, string intuityToken)
        {
            ProcessResponse processResponse = new ProcessResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    string url = !string.IsNullOrEmpty(intuityURL) ? intuityURL : IntuityAPIURL;
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", intuityToken);

                    HttpResponseMessage response = new HttpResponseMessage();

                    if (request.GetType() == typeof(IntuityEligibilityProfile))
                    {
                        IntuityEligibilityProfile eligibilityProfile = request as IntuityEligibilityProfile;
                        response = await client.PutAsJsonAsync("/api/v1/patients/" + patient_unique_id, eligibilityProfile);
                    }
                    else if (request.GetType() == typeof(IntuityEligibilityStatus))
                    {
                        IntuityEligibilityStatus intuityEligibilityStatus = request as IntuityEligibilityStatus;
                        response = await client.PutAsJsonAsync("/api/v1/patients/" + patient_unique_id, intuityEligibilityStatus);
                    }
                    else if (request.GetType() == typeof(Replenishment))
                    {
                        Replenishment replenishment = request as Replenishment;
                        response = await client.PutAsJsonAsync("/api/v1/patients/" + patient_unique_id, replenishment);
                    }
                    else if (request.GetType() == typeof(QuantityOnHand))
                    {
                        QuantityOnHand qoh = request as QuantityOnHand;
                        response = await client.PutAsJsonAsync("/api/v1/patients/" + patient_unique_id, qoh);
                    }
                    else if (request.GetType() == typeof(OptingOut))
                    {
                        OptingOut optingOut = request as OptingOut;
                        response = await client.PutAsJsonAsync("/api/v1/patients/" + patient_unique_id, optingOut);
                    }

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

        private static async Task<IntuityEligibilityLogAPIResponse> GetIntuityEligibility(string patient_unique_id, string intuityURL, string intuityToken)
        {
            IntuityEligibilityLogAPIResponse processResponse = new IntuityEligibilityLogAPIResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    string url = !string.IsNullOrEmpty(intuityURL) ? intuityURL : IntuityAPIURL;
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", intuityToken);

                    HttpResponseMessage response = await client.GetAsync("/api/v1/patients/" + patient_unique_id);

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse.intuityAPIResponse = JsonConvert.DeserializeObject<IntuityAPIResponse>(apiJsonResponse);
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

        private static async Task<IntuityEnrolmentURLResponse> GetIntuityEnrolmentURL(string patient_unique_id, bool deliver_email, string intuityURL, string intuityToken)
        {
            IntuityEnrolmentURLResponse processResponse = new IntuityEnrolmentURLResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    string url = !string.IsNullOrEmpty(intuityURL) ? intuityURL : IntuityAPIURL;
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", intuityToken);

                    HttpResponseMessage response = await client.GetAsync("/api/v1/patients/" + patient_unique_id + "/enrolment_link?deliver_email=" + deliver_email.ToString().ToLower());

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<IntuityEnrolmentURLResponse>(apiJsonResponse);
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

        private static async Task<SendPatternDetailsResponse> SendPatternsDetails(object request, string intuityURL, string intuityToken)
        {
            SendPatternDetailsResponse processResponse = new SendPatternDetailsResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    string url = !string.IsNullOrEmpty(intuityURL) ? intuityURL : IntuityAPIURL;
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", intuityToken);

                    HttpResponseMessage response = new HttpResponseMessage();

                    if (request.GetType() == typeof(PatternSync))
                    {
                        PatternSync patternSync = request as PatternSync;
                        response = await client.PutAsJsonAsync("/api/v1/patterns/" + patternSync.guid, patternSync);
                    }
                    else if (request.GetType() == typeof(PatternPairing))
                    {
                        PatternPairing patternParing = request as PatternPairing;
                        response = await client.PutAsJsonAsync("/api/v1/patterns/" + patternParing.guid, patternParing);
                    }
                    else if (request.GetType() == typeof(PatternCreation))
                    {
                        PatternCreation patternCreation = request as PatternCreation;
                        response = await client.PostAsJsonAsync("/api/v1/patterns", patternCreation);
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse = JsonConvert.DeserializeObject<SendPatternDetailsResponse>(apiJsonResponse);
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

        private static async Task<ProcessResponse> PostVerifyUser(VerifyIntuityUserRequest request)
        {
            ProcessResponse processResponse = new ProcessResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(IntuityURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", AuthToken);

                    // New code:
                    HttpResponseMessage response = await client.PutAsJsonAsync("/api/v1/user_verifications", request);
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

        private static async Task<IntuityRefreshTokenResponse> RefreshIntuityToken(string refreshToken)
        {
            IntuityRefreshTokenResponse processResponse = new IntuityRefreshTokenResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(IntuityAPIURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", refreshToken);

                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/tokens", "");
                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse.refreshToken = JsonConvert.DeserializeObject<IntuityRefreshTokenResponse.RefreshToken>(apiJsonResponse);
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
