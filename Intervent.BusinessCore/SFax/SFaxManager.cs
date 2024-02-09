using Intervent.HWS;
using Intervent.Web.DataLayer;
using Newtonsoft.Json;
using NLog;
using System.Net;
using System.Net.Http.Headers;
using System.Xml.Linq;
using static Intervent.Business.SFax.SFaxUtility;

namespace Intervent.Business.SFax
{
    public class SFaxManager : BaseManager
    {
        public SFaxResponse SendFax(MemoryStream pdfFile, string recipientName, string recipientFax, int recipientId, string reportName, string sFaxAPIUrl, string sFaxAPIKey, string sFaxUsername)
        {
            SFaxResponse sFaxResponse = new SFaxResponse();
            try
            {
                //SendFax 
                var message = new HttpRequestMessage();
                var content = new MultipartFormDataContent();
                string EncryptionKey = "tRyV4cqKsS3TsxNznDh4SX_e_$)G@ik*";
                string EncryptionInitVector = "x49e*wJVXr8BrALE";

                string token = AESCrypto.GenerateSecurityTokenUrl(sFaxUsername, sFaxAPIKey, EncryptionKey, EncryptionInitVector, false);

                var fileName = recipientId + "_" + reportName + "_" + DateTime.Now.ToString("_ddMMyyhhmmssFFF") + ".PDF";
                content.Add(new StreamContent(pdfFile), "file", fileName);
                //construct URL
                string methodSignature = "SendFax";
                // Construct the base service URL endpoint
                string url = string.Concat
                (
                    sFaxAPIUrl,
                    WebUtility.UrlEncode(methodSignature),
                    "?",
                    "token=", WebUtility.UrlEncode(token),
                    "&apikey=", WebUtility.UrlEncode(sFaxAPIKey),
                    "&RecipientName=", WebUtility.UrlEncode(recipientName),
                    "&RecipientFax=", WebUtility.UrlEncode(recipientFax)
                );
                url = url + "&";
                Console.WriteLine("URL: " + url);
                message.Method = HttpMethod.Post;
                message.Content = content;
                message.RequestUri = new Uri(url);
                message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(300000) };
                var task = client.SendAsync(message).ContinueWith((t) => ResponseFinished(t.Result));

                task.Wait();
                return task.Result;
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "SFax-SendFax", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
                sFaxResponse.message = ex.Message;
                return sFaxResponse;
            }
        }

        private SFaxResponse ResponseFinished(HttpResponseMessage response)
        {
            var task = response.Content.ReadAsStringAsync().ContinueWith<string>(o =>
            {
                return o.Result;
            });
            task.Wait();

            LogReader reader = new LogReader();
            LogLevel logLevel = LogLevel.Info;
            string message = string.Format("Request: {0}, Response: {1}", JsonConvert.SerializeObject(response.RequestMessage), JsonConvert.SerializeObject(task.Result));
            var logEvent = new LogEventInfo(logLevel, "SFax-SendFax", null, message, null, null);
            reader.WriteLogMessage(logEvent);

            SFaxResponse sFaxResponse = new SFaxResponse();
            XElement contacts = XElement.Parse(task.Result);
            sFaxResponse.isSuccess = Convert.ToBoolean(contacts.Elements("isSuccess").FirstOrDefault().Value);
            sFaxResponse.message = contacts.Elements("message").FirstOrDefault().Value;
            return sFaxResponse;
        }
    }
}
