namespace Intervent.HWS
{
    using Intervent.Utils;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public static class MediOrbis
    {
        public static ProcessResponse SendUserProgramStatus(MediOrbisRequest request, string mediOrbisURL, string mediOrbisSecretKey)
        {
            var modelResponse = Task.Run(() => PostUserProgramStatus(request, mediOrbisURL, mediOrbisSecretKey));
            return modelResponse.Result;
        }

        private static async Task<ProcessResponse> PostUserProgramStatus(MediOrbisRequest request, string mediOrbisURL, string mediOrbisSecretKey)
        {
            ProcessResponse processResponse = new ProcessResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(mediOrbisURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("secret", mediOrbisSecretKey);

                    HttpResponseMessage response = await client.PostAsJsonAsync("/msmd-support/selfCareToolStatusUpdate", request);
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
    }
}
