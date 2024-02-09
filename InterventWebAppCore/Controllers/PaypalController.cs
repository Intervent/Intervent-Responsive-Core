using Intervent.Web.DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace InterventWebApp
{
    public class PaypalController : Controller
    {
        [Authorize]
        [RequireHttps]
        public ActionResult PaymentWithPaypal(int? progId, int? user, string progName, float? cost)
        {
            //getting the apiContext as earlier
            APIContext apiContext = PayPalConfiguration.GetAPIContext();
            LogReader reader = new LogReader();
            try
            {
                string payerId = HttpContext.Request.Query["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist
                    // Creating a payment
                    var loginId = (HttpContext.Session.GetString(SessionContext.UserId)).ToString();
                    var hraId = (HttpContext.Session.GetString(SessionContext.HRAId)).ToString();
                    var lang = (HttpContext.Session.GetString(SessionContext.LanguagePreference));
                    string baseURI = Request.Url.Scheme + "://" + Request.Host.Value + "/Paypal/PaymentWithPayPal?userId=" + user + "&progId=" + progId + "&loginId=" + loginId + "&hraId=" + hraId + "&lang=" + lang + "&";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment
                    if (user != null)
                    {
                        var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, progName, cost.Value, progId.ToString(), user.Value.ToString());
                        var links = createdPayment.links.GetEnumerator();
                        string paypalRedirectUrl = null;
                        while (links.MoveNext())
                        {
                            Links lnk = links.Current;
                            if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                            {
                                paypalRedirectUrl = lnk.href;
                            }
                        }
                        // saving the paymentID in the key guid
                        HttpContext.Session.Add(guid, createdPayment.id);
                        return Json(new { url = paypalRedirectUrl });
                    }
                    else
                    {
                        return RedirectToAction("../Program/CurrentPrograms");
                    }
                }
                else
                {
                    // This section is executed when we have received all the payments parameters
                    var guid = HttpContext.Request.Query["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, HttpContext.Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        var logEvent = new LogEventInfo(NLog.LogLevel.Info, "PayPal", null, "Transaction failed " + executedPayment.failure_reason, null, null);
                        reader.WriteLogMessage(logEvent);
                        return RedirectToAction("CurrentPrograms", "Program", new { failed = true });
                    }
                    var userId = Convert.ToInt32(HttpContext.Request.Query["userId"]);
                    var loginid = Convert.ToInt32(HttpContext.Request.Query["loginId"]);
                    var hraId = Convert.ToInt32(HttpContext.Request.Query["hraId"]);
                    var programId = Convert.ToInt32(HttpContext.Request.Query["progId"]);
                    var lang = HttpContext.Request.Query["lang"];
                    var response = ProgramUtility.EnrollinProgram(userId, programId, hraId, null, loginid, lang, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetInt32(SessionContext.IntegrationWith).HasValue ? (byte)HttpContext.Session.GetInt32(SessionContext.IntegrationWith).Value : null);
                    reader.AddTransactionDetails(new Intervent.Web.DTO.AddPaymentTransactionRequest { UserId = userId, TransactionId = executedPayment.id, RelatedId = response.ProgramsInPortalId, Type = "Program" });
                    HttpContext.Session.SetString(SessionContext.UserinProgramId, response.UsersinProgramId.ToString());
                    HttpContext.Session.SetString(SessionContext.ProgramsInPortalId, response.ProgramsInPortalId.ToString());
                    HttpContext.Session.SetString(SessionContext.ProgramType, response.ProgramType.ToString());
                    return RedirectToAction("../Program/MyProgram/");
                }
            }
            catch (Exception)
            {
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "PayPal", null, "Transaction failed " + DateTime.Now.ToString(), null, null);
                reader.WriteLogMessage(logEvent);
                return Json(new { url = "../Program/CurrentPrograms?failed=true" });
            }
        }
        private PayPal.Api.Payment payment;
        [Authorize]
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        [Authorize]
        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string progName, float cost, string progId, string userId)
        {
            var itemList = new ItemList() { items = new List<Item>() };
            itemList.items.Add(new Item()
            {
                name = progName,
                currency = "USD",
                price = cost.ToString(),
                quantity = "1"
            });

            var payer = new Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };
            var amount = new Amount()
            {
                currency = "USD",
                total = cost.ToString()
            };
            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "Transaction description.",
                invoice_number = userId + progId + DateTime.UtcNow.ToShortDateString().Replace("/", ""),
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            return this.payment.Create(apiContext);
        }

    }
}