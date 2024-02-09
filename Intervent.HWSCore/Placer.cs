using Intervent.Utils;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using static Intervent.HWS.PlacerRequest;
using static Intervent.HWS.PullReportListResponse;

namespace Intervent.HWS
{
    public static class Placer
    {
        public static PullReportListResponse PullReportList(GetExternalReportsList request, string teamsBPApiKey, string teamsBPURL)
        {
            var modelResponse = Task.Run(() => ProcessReportList(request, teamsBPApiKey, teamsBPURL));
            return modelResponse.Result;
        }

        public static PullReportListResponse RetrieveReport(GetExternalReports request, string teamsBPApiKey, string teamsBPURL)
        {
            var modelResponse = Task.Run(() => RetrieveExternalReport(request, teamsBPApiKey, teamsBPURL));
            return modelResponse.Result;
        }

        public static PullReportListResponse GetRPMSummaryGraph(GetRPMSummaryGraph request, string teamsBPApiKey, string teamsBPURL)
        {
            var modelResponse = Task.Run(() => RetrieveRPMSummaryGraph(request, teamsBPApiKey, teamsBPURL));
            return modelResponse.Result;
        }

        public static PostCoachingResponse PostCoachingNotes(PPRFormRequest request, string teamsBPApiKey, string teamsBPURL)
        {
            var modelResponse = Task.Run(() => ProcessCoachingNotes(request, teamsBPApiKey, teamsBPURL));
            return modelResponse.Result;
        }

        private static async Task<PostCoachingResponse> ProcessCoachingNotes(PPRFormRequest request, string teamsBPApiKey, string teamsBPURL)
        {
            PostCoachingResponse processResponse = new PostCoachingResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(teamsBPURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync(string.Format("/strokeAPI/coachingNotes?apiKey={0}", teamsBPApiKey), request);

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse.Response = apiJsonResponse;
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
        private static async Task<PullReportListResponse> ProcessReportList(GetExternalReportsList request, string teamsBPApiKey, string teamsBPURL)
        {
            PullReportListResponse processResponse = new PullReportListResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(teamsBPURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(string.Format("/strokeAPI/report?apiKey={0}&tenant={1}&act=getReportList&study_subject_ID={2}", teamsBPApiKey, request.tenant, request.uniqueId));

                    if (response.IsSuccessStatusCode)
                    {
                        var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                        processResponse.ParticipantReportList = JsonConvert.DeserializeObject<PullReportList>(apiJsonResponse);
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

        private static async Task<PullReportListResponse> RetrieveExternalReport(GetExternalReports request, string teamsBPApiKey, string teamsBPURL)
        {
            PullReportListResponse processResponse = new PullReportListResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(teamsBPURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(string.Format("/strokeAPI/report?apiKey={0}&tenant={1}&act=getPatientReport&study_subject_ID={2}&report_name={3}", teamsBPApiKey, request.tenant, request.uniqueId, request.reportName));

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] apiJsonResponse = response.Content.ReadAsByteArrayAsync().Result;
                        processResponse.ExternalReportData = new ExternalReport();
                        processResponse.ExternalReportData.ReportData = apiJsonResponse;
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

        private static async Task<PullReportListResponse> RetrieveRPMSummaryGraph(GetRPMSummaryGraph request, string teamsBPApiKey, string teamsBPURL)
        {
            PullReportListResponse processResponse = new PullReportListResponse();
            try
            {
                using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                {
                    client.BaseAddress = new Uri(teamsBPURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(string.Format("/wearable/summarygraphic/coach?apiKey={0}&study_subject_ID={1}&start_date={2}&end_date={3}", teamsBPApiKey, request.uniqueId, request.startDate.Date.ToString("yyyy-MM-dd"), request.endDate.Date.ToString("yyyy-MM-dd")));

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] apiJsonResponse = response.Content.ReadAsByteArrayAsync().Result;
                        processResponse.RPMGraphData = new RPMGraph();
                        processResponse.RPMGraphData.GraphData = apiJsonResponse;
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
