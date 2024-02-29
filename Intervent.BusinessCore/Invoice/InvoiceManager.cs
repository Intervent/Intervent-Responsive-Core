using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using IronPdf;
using NLog;
using System.Configuration;

namespace Intervent.Business
{
    public class InvoiceManager : BaseManager
    {
        ChromePdfRenderer Renderer = new ChromePdfRenderer();
        ReportReader reportReader = new ReportReader();
        public static string BaseUrl = ConfigurationManager.AppSettings["EmailUrl"].ToString();
        public string InvoicePath;

        public void ProcessInvoiceBilling()
        {
            LogReader logReader = new LogReader();
            try
            {
                reportReader.AddMonitorChargeToInvoiceDetails();
                reportReader.AddIntuityEventToInvoiceDetails();
                var invoiceList = reportReader.GetAllPendingInvoiceBill();
                if (invoiceList.InvoiceDetails.Count > 0)
                {
                    int count = 0;
                    DateTime date = DateTime.Now.AddMonths(-1);
                    var invoiceStartDate = new DateTime(date.Year, date.Month, 1);
                    var invoiceEndDate = invoiceStartDate.AddMonths(1).AddDays(-1);
                    string invoiceRootPath = ConfigurationManager.AppSettings["InvoicePath"];
                    string invoiceMonth = invoiceStartDate.ToString("MMMM yyyy");
                    InvoicePath = invoiceRootPath + invoiceMonth;
                    License.LicenseKey = ConfigurationManager.AppSettings["IronPdf.LicenseKey"];
                    var MyTempPath = ConfigurationManager.AppSettings["PDFReportPath"];
                    Environment.SetEnvironmentVariable("TEMP", MyTempPath);
                    Environment.SetEnvironmentVariable("TMP", MyTempPath);
                    Installation.TempFolderPath = Path.Combine(MyTempPath, "IronPdfTemp");

                    if (!Directory.Exists(invoiceRootPath))
                        Directory.CreateDirectory(invoiceRootPath);

                    if (!Directory.Exists(InvoicePath))
                        Directory.CreateDirectory(InvoicePath);

                    Renderer.RenderingOptions.MarginTop = 6;
                    Renderer.RenderingOptions.MarginBottom = 18;
                    Renderer.RenderingOptions.MarginLeft = 5;
                    Renderer.RenderingOptions.MarginRight = 5;
                    Renderer.RenderingOptions.CssMediaType = IronPdf.Rendering.PdfCssMediaType.Print;
                    Renderer.RenderingOptions.PrintHtmlBackgrounds = true;
                    Renderer.RenderingOptions.EnableJavaScript = true;
                    Renderer.RenderingOptions.Timeout = 120; // seconds 
                    Renderer.RenderingOptions.RenderDelay = 2000; //milliseconds=10seconds

                    foreach (InvoiceBilledDetailsDto invoiceBill in invoiceList.InvoiceDetails)
                    {
                        InvoiceModel model = new InvoiceModel
                        {
                            InvoiceList = invoiceBill.InvoiceDetails.ToList(),
                            InvoiceDate = DateTime.UtcNow,
                            InvoiceStartDate = invoiceStartDate,
                            InvoiceEndDate = invoiceEndDate,
                            InvoiceNo = invoiceBill.Id.ToString(),
                            User = invoiceBill.User,
                            Participant = invoiceBill.User,
                            Total = invoiceBill.Total
                        };
                        if (GenerateInvoice(model))
                        {
                            reportReader.EditInvoiceDetails(new InvoiceDetailsRequest { Id = invoiceBill.Id });
                            count++;
                        }
                    }
                    var logEvent = new LogEventInfo(LogLevel.Trace, "InvoiceManager", null, "Invoice created. Total (" + count + ")", null, null);
                    logReader.WriteLogMessage(logEvent);
                }
            }
            catch (Exception ex)
            {
                var logEvent = new LogEventInfo(LogLevel.Error, "InvoiceManager.ProcessInvoiceBilling", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
        }

        public class InvoiceModel
        {
            public List<InvoiceDetailsDto> InvoiceList { get; set; }

            public UserDto User { get; set; }

            public UserDto Participant { get; set; }

            public string InvoiceNo { get; set; }

            public decimal Total { get; set; }

            public DateTime InvoiceDate { get; set; }

            public DateTime InvoiceStartDate { get; set; }

            public DateTime InvoiceEndDate { get; set; }
        }

        private bool GenerateInvoice(InvoiceModel model)
        {
            try
            {
                Renderer.RenderHtmlAsPdf(RenderInvoice(model), BaseUrl).SaveAs(InvoicePath + "\\" + model.User.UniqueId + "_" + DateTime.UtcNow.ToString("yyyyMMdd") + ".pdf");
                return true;
            }
            catch (Exception e)
            {
                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "InvoiceManager.GenerateInvoice", null, "Invoice generation failed for user id " + model.User.Id, null, e);
                reader.WriteLogMessage(logEvent);
                return false;
            }
        }

        private string RenderInvoice(InvoiceModel model)
        {
            string html = "<html><link href=\"" + BaseUrl + "/Content/css/invoice.css\" rel=\"stylesheet\" type=\"text/css\" />";
            html = html + "<div class=\"grid-container\">";
            html = html + "<div class=\"grid-x\">";
            html = html + "<div class=\"cell invoice-title\">";
            html = html + "<p>Credible, Trusted, Proven</p>";
            html = html + "<p>www.interventhealth.com</p>";
            html = html + "</div><div class=\"cell invoice-header\">";
            html = html + "<div class=\"grid-x\">";
            html = html + "<div class=\"cell small-7 invoice-company-name\">";
            html = html + "<h1>INTERVENT INTERNATIONAL, LLC</h1></div>";
            html = html + "<div class=\"cell small-5 invoice-company-logo\">";
            html = html + "<img src=\"" + BaseUrl + "/Images/interventHealthLogo.png\" alt=\"Intervent Health\" /></div>";
            html = html + "<div class=\"cell small-7\">";
            html = html + "<p>340 Eisenhower Drive; Building 1400</p>";
            html = html + "<p>Savannah, GA 31406</p>";
            html = html + "<p>Tel: 912 349 2336</p>";
            html = html + "<p>www.interventhealth.com</p>";
            html = html + "<p>&nbsp;</p><p>Federal Tax ID# = 45-2954871</p></div>";
            html = html + "<div class=\"cell small-5\">";
            html = html + "<p>BILL TO ATTENTION:</p>";
            html = html + "<p>Eben Concepts</p>";
            html = html + "<p>639 Executive Place, Suite 202</p>";
            html = html + "<p>Fayetteville, NC 29305</p>";
            if (model.User.ContactMode.HasValue)
            {
                if (model.User.ContactMode.Value == 1 && !string.IsNullOrEmpty(model.User.HomeNumber))
                {
                    html = html + "<p>Tel: " + model.User.HomeNumber + "</p>";
                }
                else if (model.User.ContactMode.Value == 2 && !string.IsNullOrEmpty(model.User.WorkNumber))
                {
                    html = html + "<p>Tel: " + model.User.WorkNumber + "</p>";
                }
                else if (model.User.ContactMode.Value == 3 && !string.IsNullOrEmpty(model.User.CellNumber))
                {
                    html = html + "<p>Tel: " + model.User.CellNumber + "</p>";
                }
            }
            html = html + "<p>&nbsp;</p>";
            html = html + "<p>Employee Name: " + model.User.FirstName + model.User.LastName + "</p>";
            html = html + "<p>Member ID: " + model.User.UniqueId + "</p>";
            html = html + "<p>Participant Name: " + model.User.FirstName + model.User.LastName + "</p>";
            html = html + "<p>Participant DOB: " + model.User.DOB.Value.ToString("MM/dd/yyyy") + "</p>";
            html = html + "</div>";
            html = html + "<div class=\"cell\">";
            html = html + "<p>Invoice Number: " + model.InvoiceNo + "</p>";
            html = html + "<p>Invoice Date: " + model.InvoiceDate.ToString("MM/dd/yyyy") + "</p>";
            html = html + "</div>";
            html = html + "</div>";
            html = html + "</div>";
            html = html + "<div class=\"cell invoice-body\">";
            html = html + "<table>";
            html = html + "<thead>";
            html = html + "<tr>";
            html = html + "<td>Date of Service</td>";
            html = html + "<td>Description of Service</td>";
            html = html + "<td>Rate</td>";
            html = html + "<td>Total</td>";
            html = html + "</tr>";
            html = html + "</thead>";
            html = html + "<tbody>";
            foreach (var invoiceDetail in model.InvoiceList)
            {
                html = html + "<tr>";
                html = html + "<td>" + model.InvoiceStartDate.ToString("MM/dd/yyyy") + " - " + model.InvoiceEndDate.ToString("MM/dd/yyyy") + " </td>";
                html = html + "<td>" + invoiceDetail.BillingServiceType.Type + "</td>";
                html = html + "<td>$" + Math.Round(invoiceDetail.BillingServiceType.Price, 2) + "</td>";
                html = html + "<td>$" + Math.Round(invoiceDetail.BillingServiceType.Price, 2) + "</td>";
                html = html + "</tr>";
            }
            html = html + "<tr>";
            html = html + "<td></td>";
            html = html + "<td>Total</td>";
            html = html + "<td></td>";
            html = html + "<td>$" + Math.Round(model.Total, 2) + "</td>";
            html = html + "</tr>";
            html = html + "</tbody>";
            html = html + "</table>";
            html = html + "</div>";
            html = html + "<div class=\"cell invoice-title\">";
            html = html + "<p>Thank You For Your Business</p>";
            html = html + "<p>www.interventhealth.com</p>";
            html = html + "</div>";
            html = html + "</div>";
            html = html + "</div>";
            html = html + "</html>";
            return html;
        }

    }
}