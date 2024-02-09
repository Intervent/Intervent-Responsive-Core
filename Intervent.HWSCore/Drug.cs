using System.Net.Http.Headers;

//Validic Health Web Service Integration
namespace Intervent.HWS
{
    public static class Drug
    {
        private const string FDAURL = "https://api.fda.gov/";

        #region Data Objects
        public static async Task<List<Results>> GetDrugDetails(String search, string apiKey)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(FDAURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync(string.Format("/drug/label.json?api_key={0}&search=openfda.brand_name:{" + 1 + "}&limit=10", apiKey, search));
                    if (response.IsSuccessStatusCode)
                    {
                        var responseObj = await response.Content.ReadAsAsync<DrugsResponse>();
                        if (responseObj != null && responseObj.meta != null)
                        {
                            return responseObj.results;
                        }
                    }
                }
            }
            catch { }
            return new List<Results>();
        }
        #endregion
    }
}
