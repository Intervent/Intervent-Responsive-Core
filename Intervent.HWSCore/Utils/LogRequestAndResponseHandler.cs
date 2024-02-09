using System.Text;

namespace Intervent.Utils
{
    /// <summary>
    /// Log Request and response
    /// </summary>
    public class LogRequestAndResponseHandler : DelegatingHandler
    {
        public LogRequestAndResponseHandler()
            : base()
        {
        }

        public LogRequestAndResponseHandler(HttpMessageHandler innerHandler)
        : base(innerHandler)
        {
        }
        /// <summary>
        /// Overrider sendasync to log request and response
        /// </summary>
        /// <param name="request">http request</param>
        /// <param name="cancellationToken"></param>
        /// <returns>response</returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var fileName = System.DateTime.Now.ToString("ddMMyyhh") + ".txt";
            // log request body
            await MessageLog.LogRequestMessage(request, fileName);

            // let other handlers process the request
            var result = await base.SendAsync(request, cancellationToken);

            // once response body is ready, log it
            await MessageLog.LogResponseMessage(result, fileName);

            return result;
        }
    }

    public static class MessageLog
    {
        public static string FilePath = System.Configuration.ConfigurationManager.AppSettings["FilePath"];
        public static async Task LogRequestMessage(HttpRequestMessage request, string fileName)
        {
            try
            {
                string requestBody;
                if (request.Content != null)
                    requestBody = await request.Content.ReadAsStringAsync();
                else
                    requestBody = request.ToString();
                requestBody = string.Format("Request({0})({2})({3}): {1}", DateTime.Now.ToString(), requestBody, request.Method, request.RequestUri.AbsoluteUri);

                await WriteToFile(requestBody, fileName);
            }
            catch { }
        }

        public static async Task LogResponseMessage(HttpResponseMessage result, string fileName)
        {
            try
            {
                string responseBody;
                if (result.Content != null)
                    responseBody = await result.Content.ReadAsStringAsync();
                else
                    responseBody = result.ToString();
                responseBody = string.Format("Response({0}): {1}", DateTime.Now.ToString(), responseBody);
                await WriteToFile(responseBody, fileName);
            }
            catch { }
        }

        public static async Task WriteToFile(string text, string filePath)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);
            filePath = FilePath + filePath;
            using (FileStream sourceStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }

    }

}