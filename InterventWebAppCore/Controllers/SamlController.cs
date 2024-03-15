using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InterventWebApp.Controllers
{
    public class SamlController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly IWebHostEnvironment _environment;

        public SamlController(IOptions<AppSettings> appSettings, IWebHostEnvironment environment)
        {
            _appSettings = appSettings.Value;
            _environment = environment;
        }

        public string debug = string.Empty;
        /*public async Task<ActionResult> Index()
        {
            return await Provider("edlogics");
        }

        [HttpPost]

        public async Task<ActionResult> Intuity(IntuityModel model)
        {
            string debug = "";
            LogReader reader = new LogReader();
            try
            {
                debug = model.authorization + "~" + model.token + "~" + model.utcdatetime;
                var logEvent1 = new LogEventInfo(NLog.LogLevel.Error, "SamlController", null, debug, null, null);
                reader.WriteLogMessage(logEvent1);
                if (AuthorizationUtility.VerifySecretKey(model.authorization, model.utcdatetime, model.token))
                {
                    var userresponse = await AccountUtility.GetValidUserByToken(model.token);
                    var response = userresponse != null ? userresponse.User : null;
                    if (response != null)
                    {
                        try
                        {
                            // Create a login context for the asserted identity.
                            FormsAuthentication.SetAuthCookie(response.UserName, false);
                        }
                        catch { }
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        var identity = await AccountUtility.CreateUserIdentity(response, DefaultAuthenticationTypes.ApplicationCookie);
                        HttpContext.Session.SetInt32(SessionContext.UserId, response.Id.ToString());
                        HttpContext.Session.SetString(SessionContext.TermsSSO, response.Organization.TermsForSSO.ToString());
                        HttpContext.Session.SetString(SessionContext.TermsAccepted, response.TermsAccepted.ToString());
                        identity.claimsIdentity.AddClaim(new Claim("UserId", response.Id.ToString()));
                        identity.claimsIdentity.AddClaim(new Claim("FullName", response.FirstName + " " + response.LastName));
                        identity.claimsIdentity.AddClaim(new Claim("UserName", response.UserName));
                        identity.claimsIdentity.AddClaim(new Claim("Module", AccountController.GetGroups(response)));
                        identity.claimsIdentity.AddClaim(new Claim("RoleCode", AccountController.GetRoleCodes(response)));
                        identity.claimsIdentity.AddClaim(new Claim("ExpirationUrl", response.Organization.Url));
                        identity.claimsIdentity.AddClaim(new Claim("SingleSignOn", "true"));
                        identity.claimsIdentity.AddClaim(new Claim("MobileSignOn", "false"));
                        if (response.TimeZoneId.HasValue)
                            identity.claimsIdentity.AddClaim(new Claim("TimeZone", CommonUtility.GetTimeZones(response.TimeZoneId).TimeZones[0].TimeZoneId));
                        AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity.claimsIdentity);
                        var logEvent = new LogEventInfo(NLog.LogLevel.Info, response.Id.ToString(), null, "Intuity User Login", null, null);
                        reader.WriteLogMessage(logEvent);
                        AccountUtility.LogUserLastAccess(response.Id, true, null);
                        return RedirectToAction("Stream", "Participant");
                    }
                }
            }
            catch (Exception ex)
            {
                debug += ex.Message + ex.StackTrace;
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "SamlController", null, debug, null, ex);
                reader.WriteLogMessage(logEvent);
            }
            //return new HttpStatusCodeResult(401);
            return RedirectToAction("NotAuthorized", "Account");
        }

        public async Task<ActionResult> Provider(string id)
        {
            SamlUtility utility = new SamlUtility();
            SSOProviderDto provider = null;
            LogReader reader = new LogReader();
            try
            {
                provider = utility.GetProviderByName(id);
                if (provider != null)
                {
                    ComponentPro.Saml2.Response samlResponse;
                    string relayState;
                    // Get the SAML response
                    debug += utility.ExtractResponse(provider, out samlResponse, out relayState);

                    if (samlResponse != null)
                    {
                        debug += "ExtractSuccess";

                        // It indicates a success or an error?
                        if (samlResponse.IsSuccess())
                        {
                            var issuer = provider != null ? provider.Issuer : SamlUtility.EdIssuer;
                            if (samlResponse.Issuer.NameIdentifier.ToLower().Equals(issuer))
                            {
                                // Process the success response.
                                return await SamlSuccessRedirect(provider, samlResponse, relayState, provider.RedirectUrl);

                            }
                            debug += "ID" + samlResponse.Issuer.NameIdentifier + "QU" + samlResponse.Issuer.NameQualifier + "SPQ" + samlResponse.Issuer.SpNameQualifier + samlResponse.Issuer.Format + "SPID" + samlResponse.Issuer.SpProvidedId;
                        }
                        else
                        {
                            debug += "IsSuccess false";

                            // Process the error response.
                            if ((samlResponse.Status.StatusMessage != null))
                            {
                                debug += samlResponse.Status.StatusMessage.Message;
                            }

                        }
                    }
                    else
                        debug += "Extract Response empty";
                }
                else
                {
                    debug = "Provider not found on our side";
                }

                reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "SamlController", null, debug, null, null));

            }

            catch (System.Exception exception)
            {
                debug += exception.Message + exception.StackTrace;
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "SamlController", null, debug, null, exception);
                reader.WriteLogMessage(logEvent);

            }

            ViewData["ErrorMessage"] = debug;

            if (provider != null && !string.IsNullOrEmpty(provider.RedirectUrl))
            {
                return Redirect(provider.RedirectUrl);
            }
            return View("Index");
        }

        //Idp logout (make sure to logout all SPs
        public ActionResult Logout()
        {
            isSp = false;
            EdlogicsLogout();
            return View("Index");
        }

        public ActionResult Login()
        {
            try
            {
                // Load the Single Sign On state from the Session state.
                // If the saved authentication state is a null reference, receive the authentication request from the query string and form data.
                SsoAuthnState ssoState = HttpContext.Session.GetString(SessionContext.SSOState) as SsoAuthnState;
                if (ssoState == null || HttpContext.Session.GetInt32(SessionContext.UserId).Value == null || string.IsNullOrEmpty(HttpContext.Session.GetInt32(SessionContext.UserId).Value))
                {
                    // Receive the authentication request.
                    AuthnRequest authnRequest;
                    string relayState;

                    debug += SamlUtility.ProcessAuthnRequest(HttpContext, out authnRequest, out relayState);

                    if (authnRequest == null)
                    {
                        // No authentication request found.
                        debug += "No Authentication request found";
                    }
                    else if (!authnRequest.Issuer.NameIdentifier.ToLower().Equals(SamlUtility.EdSPIssuer))
                    {
                        debug += "Issuer mismatch" + authnRequest.Issuer.NameIdentifier;
                    }
                    else
                    {
                        debug += "ProcessAuthnRequest success";
                        // Process the authentication request.
                        bool forceAuthn = authnRequest.ForceAuthn;
                        bool allowCreate = false;

                        if (authnRequest.NameIdPolicy != null)
                        {
                            debug += "NameIdPolicy exist";
                            allowCreate = authnRequest.NameIdPolicy.AllowCreate;
                        }

                        ssoState = new SsoAuthnState();
                        ssoState.AuthnRequest = authnRequest; ;
                        ssoState.RelayState = relayState;
                        ssoState.IdpProtocolBinding = SamlBindingUri.UriToBinding(authnRequest.ProtocolBinding);
                        ssoState.AssertionConsumerServiceURL = authnRequest.AssertionConsumerServiceUrl;
                        //
                        if (HttpContext != null && HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
                        {
                            var userId = Convert.ToInt32(HttpContext.Convert.ToInt32(User.UserId()));
                            debug += "IDUserId" + userId;
                            HttpContext.Session.SetInt32(SessionContext.ParticipantId, userId.ToString());
                            var samlPartResponse = ParticipantUtility.ReadUserParticipation(userId);
                            HttpContext.Session.SetString(SessionContext.ParticipantName, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(samlPartResponse.user.FirstName.ToLower()) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(samlPartResponse.user.LastName.ToLower()));
                            HttpContext.Session.SetString(SessionContext.ParticipantEmail, samlPartResponse.user.Email);
                            HttpContext.Session.SetString(SessionContext.DOB, samlPartResponse.user.DOB.ToString());
                            if (samlPartResponse.user.Gender.HasValue)
                                HttpContext.Session.SetInt32(SessionContext.Gender, samlPartResponse.user.Gender.Value);
                            HttpContext.Session.SetString(SessionContext.Zip, samlPartResponse.user.Zip);
                        }
                        else
                        {
                            debug += "IsAuthentidated" + Request.IsAuthenticated;
                            debug += "URL" + Request.Path.Value;

                            var logEvent = new LogEventInfo(NLog.LogLevel.Error, "SamlController", null, debug, null, null);
                            LogReader reader = new LogReader();

                            reader.WriteLogMessage(logEvent);
                            // Save the SSO state.
                            HttpContext.Session.SetString(SessionContext.SSOState, ssoState.ToString());
                            var url = Url.Action("Index", "Home");
                            // Initiate a local login.
                            return Redirect(url);
                        }
                    }
                }
                debug += "response";
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.SSOState)))
                    HttpContext.Session.Remove(SessionContext.SSOState);
                HttpContext.Session.SetString(SessionContext.HasSP, Boolean.TrueString);
                // Create a SAML response with the user's local identity, if any.
                ComponentPro.Saml2.Response samlResponse = SamlUtility.BuildResponse(HttpContext);

                // Send the SAML response to the service provider.
                SamlUtility.SendResponse(HttpContext, samlResponse, ssoState);

                return null;
            }
            catch (Exception exception)
            {
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "SamlController", null, debug, null, exception);
                LogReader reader = new LogReader();

                reader.WriteLogMessage(logEvent);
                debug += exception.Message;
                debug += exception.StackTrace;
            }

            ViewData["ErrorMessage"] = debug;
            return View("Index");

        }

        public ActionResult EdlogicsSP()
        {
            HttpContext.Session.SetString(SessionContext.HasSP, Boolean.TrueString);

            // Create a SAML response with the user's local identity, if any.
            ComponentPro.Saml2.Response samlResponse = SamlUtility.BuildResponse(HttpContext);

            // Send the SAML response to the service provider.
            SamlUtility.SendResponse(HttpContext, samlResponse, null);

            return null;
        }

        bool? isSp = null;
        //Edlogics logout
        public ActionResult EdlogicsLogout()
        {
            try
            {
                if (!isSp.HasValue)
                    isSp = true;
                // Get the previously loaded certificate.
                X509Certificate2 x509Certificate = (X509Certificate2)HttpContext.Application[Constants.EdlogicCertificate];

                LogoutResponse logoutResponse;
                LogoutRequest logoutRequest;

                SamlMessageUtil.CreateLogoutMessage(Request, out logoutResponse, out logoutRequest, x509Certificate.PublicKey.Key);

                if (logoutRequest != null)
                    HandleLogoutRequest(logoutRequest, x509Certificate, isSp.Value);
                else
                    HandleLogoutResponse(logoutResponse);
            }

            catch (System.Exception exception)
            {
                debug += exception.Message;
                if (exception.InnerException != null)
                    debug += exception.InnerException.Message;
            }
            ViewData["ErrorMessage"] = debug;
            return View("Index"); ;
        }


        // GET: /SingleLogoutService/
        public ActionResult SendLogoutRequest()
        {
            try
            {
                LogoutRequest logoutRequest = new LogoutRequest();
                debug += "Request creation";

                // Create a logout request

                logoutRequest.Issuer = new Issuer(SamlUtility.IssuerUrl);
                logoutRequest.NameId = new NameId("test-intervent@edlogics.com");
                #region Send Logout Response
                debug += logoutRequest.NameId;
                // Send the logout response over HTTP redirect.
                string logoutUrl = SamlUtility.EdlogicIdpUrl;
                var x509Certificate = (X509Certificate2)HttpContext.Application[Constants.InterventCertificate];
                debug += logoutUrl;
                logoutRequest.Redirect(Response, logoutUrl, logoutRequest.RelayState, x509Certificate.PrivateKey);

                return null;

                #endregion
            }
            catch (Exception exception)
            {
                debug += exception.Message;
            }
            ViewData["ErrorMessage"] = debug;
            return View("Index");
        }

        #region Saml Utility Methods
        // GET: Saml       
        /// <summary>
        /// Processes a successful SAML response.
        /// </summary>
        private async Task<ActionResult> SamlSuccessRedirect(SSOProviderDto provider, ComponentPro.Saml2.Response samlResponse, string relayState, string url)
        {
            debug = string.Empty;
            LogReader reader = new LogReader();
            // Extract the asserted identity from the SAML response.
            Assertion samlAssertion = (Assertion)samlResponse.Assertions[0];

            // Get the subject name identifier.
            string userName = samlAssertion.Subject.NameId.NameIdentifier;
            string firstName, lastName, postalCode, dob, gender, phone, email, uniqueid;
            SamlUtility utility = new SamlUtility();
            gender = dob = postalCode = firstName = lastName = phone = email = uniqueid = "";
            debug += userName;
            if (samlAssertion.AttributeStatements != null)
            {
                foreach (var att in samlAssertion.AttributeStatements)
                {
                    foreach (ComponentPro.Saml2.Attribute a in att.Attributes)
                    {
                        var name = a.Name;
                        if (!string.IsNullOrEmpty(a.FriendlyName))
                        {
                            name = a.FriendlyName;
                        }
                        //debug += a.FriendlyName;
                        debug += name;
                        if (name.ToLower().Equals(utility.GetType(AttributeType.GivenName, provider)))
                        {
                            firstName = FindValue(a);
                        }
                        else if (name.ToLower().Equals(utility.GetType(AttributeType.SurName, provider)))
                        {
                            lastName = FindValue(a);
                        }
                        else if (name.ToLower().Contains(utility.GetType(AttributeType.Postal, provider)))
                        {
                            postalCode = FindValue(a);
                        }
                        else if (a.Name.ToLower().Equals(utility.GetType(AttributeType.Gender, provider)))
                        {
                            gender = FindValue(a);
                        }
                        else if (name.ToLower().Equals(utility.GetType(AttributeType.BirthDate, provider)))
                        {
                            dob = FindValue(a);
                        }
                        else if (name.ToLower().Equals(utility.GetType(AttributeType.Phone, provider)))
                        {
                            phone = FindValue(a);
                        }
                        else if (a.Name.ToLower().Equals(utility.GetType(AttributeType.Email, provider)))
                        {
                            email = FindValue(a);
                        }
                        else if (a.Name.ToLower().Equals(utility.GetType(AttributeType.UniqueId, provider)))
                        {
                            uniqueid = FindValue(a);
                        }
                    }
                }
            }

            debug += "username:" + userName + "firstname:" + firstName + "lastname" + lastName + "email" + email + "uniqueid" + uniqueid;
            // Redirect to the originally requested resource URL.
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                try
                {
                    UserDto response = null;
                    int orgId = 0;
                    if (provider != null)
                    {
                        orgId = provider.OrganizationId;
                    }
                    else
                    {
                        //var organizationResponse = PortalUtility.ReadOrganization("Edlogics", null);
                        orgId = 56; //Edlogics Organization ID = 56
                    }
                    if (provider == null || !provider.HasEligibility)
                    {
                        var userresponse = await AccountUtility.GetUser(userName, null, null, false, null);
                        response = userresponse != null ? userresponse.User : null;

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(uniqueid))
                        {
                            userName = uniqueid;
                        }
                        var regResponse = AccountUtility.CheckifRegistered(userName, orgId);
                        if (regResponse.recordExist == true)
                        {
                            var user = await AccountUtility.GetUser(regResponse.User.UserName, null, null, false, null);
                            response = user != null ? user.User : null;
                        }

                    }


                    #region New User
                    if (response == null)
                    {

                        EligibilityDto eligibility = null;
                        if (utility.CanCreateUserProfile(provider, uniqueid, out eligibility))
                        {
                            debug += orgId;
                            if (eligibility != null)
                            {
                                var Organization = PortalUtility.ReadOrganization(null, orgId);
                                AccountController controller = new AccountController();
                                if (string.IsNullOrEmpty(email))
                                {
                                    email = uniqueid + orgId + "@samlnoemail.com";
                                }
                                var terms = (Organization.organization.TermsForSSO.HasValue && Organization.organization.TermsForSSO.Value) ? false : true;
                                var accountResponse = await controller.CreateAccount(eligibility, email, CommonUtility.GeneratePassword(10, 3), false, false, terms);
                                if (accountResponse != null && accountResponse.Succeeded)
                                {
                                    var userresponse = await AccountUtility.GetUser(email, null, null, false, null);
                                    response = userresponse != null ? userresponse.User : null;
                                }
                            }
                            else
                            {
                                CreateUserModel request = new CreateUserModel();
                                UserDto user = new UserDto();
                                user.Email = userName;
                                user.PasswordHash = CommonUtility.GeneratePassword(10, 3);
                                user.FirstName = firstName;
                                user.LastName = lastName;
                                user.UserName = userName;
                                user.PhoneNumber = phone;
                                if (!string.IsNullOrEmpty(gender))
                                {
                                    var values = utility.GetPossibleValues(AttributeType.Gender, provider);
                                    if (gender.ToLower().Equals(values[0]))
                                    {
                                        user.Gender = 1;
                                    }
                                    else if (gender.ToLower().Equals(values[1]))
                                    {
                                        user.Gender = 2;
                                    }
                                }
                                if (!string.IsNullOrEmpty(dob))
                                {
                                    user.DOB = DateTime.Parse(dob);
                                }
                                if (!string.IsNullOrEmpty(postalCode))
                                {
                                    user.Zip = postalCode;
                                }
                                user.OrganizationId = orgId;
                                user.EmailConfirmed = false;
                                user.TermsAccepted = true;
                                request.user = user;
                                request.rootPath = _webHostEnvironment.WebRootPath;
                                request.InfoEmail = _appSettings.InfoEmail;
                                request.SecureEmail = _appSettings.SecureEmail;
                                request.SMPTAddress = _appSettings.SMPTAddress;
                                request.PortNumber = _appSettings.PortNumber;
                                request.SecureEmailPassword = _appSettings.SecureEmailPassword;
                                var User = await AccountUtility.CreateUser(request);
                                var userresponse = await AccountUtility.GetUser(userName, null, null, false, null);
                                response = userresponse != null ? userresponse.User : null;
                            }
                        }
                    }
                    #endregion

                    if (response != null)
                    {
                        try
                        {
                            // Create a login context for the asserted identity.
                            FormsAuthentication.SetAuthCookie(response.UserName, false);
                        }
                        catch { }
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        var identity = await AccountUtility.CreateUserIdentity(response, DefaultAuthenticationTypes.ApplicationCookie);
                        HttpContext.Session.SetInt32(SessionContext.UserId, response.Id.ToString());
                        HttpContext.Session.SetString(SessionContext.TermsSSO, response.Organization.TermsForSSO.ToString());
                        HttpContext.Session.SetString(SessionContext.TermsAccepted, response.TermsAccepted.ToString());
                        identity.claimsIdentity.AddClaim(new Claim("UserId", response.Id.ToString()));
                        identity.claimsIdentity.AddClaim(new Claim("FullName", response.FirstName + " " + response.LastName));
                        identity.claimsIdentity.AddClaim(new Claim("UserName", response.UserName));
                        identity.claimsIdentity.AddClaim(new Claim("Module", AccountController.GetGroups(response)));
                        identity.claimsIdentity.AddClaim(new Claim("RoleCode", AccountController.GetRoleCodes(response)));
                        identity.claimsIdentity.AddClaim(new Claim("ExpirationUrl", response.Organization.Url));
                        identity.claimsIdentity.AddClaim(new Claim("SingleSignOn", "true"));
                        identity.claimsIdentity.AddClaim(new Claim("MobileSignOn", "false"));
                        if (response.TimeZoneId.HasValue)
                            identity.claimsIdentity.AddClaim(new Claim("TimeZone", CommonUtility.GetTimeZones(response.TimeZoneId).TimeZones[0].TimeZoneId));
                        AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity.claimsIdentity);
                        if (provider != null && !string.IsNullOrEmpty(provider.LogoutUrl))
                        {
                            HttpContext.Session.SetString(SessionContext.LandingPage, provider.LogoutUrl);
                        }
                        else
                        {
                            HttpContext.Session.SetString(SessionContext.HasSP, Boolean.TrueString);
                        }
                        var logEvent = new LogEventInfo(NLog.LogLevel.Info, response.Id.ToString(), null, "SSO User Login", null, null);
                        reader.WriteLogMessage(logEvent);
                        return RedirectToAction("Stream", "Participant");
                        //return RedirectToAction("Terms", "Participant");
                    }
                }
                catch (Exception ex)
                {
                    debug += "Exception:" + ex.Message + ex.InnerException;
                    var logEvent = new LogEventInfo(NLog.LogLevel.Error, "SamlController.Error", null, debug, null, ex);
                    reader.WriteLogMessage(logEvent);
                }
            }
            ViewData["ErrorMessage"] = debug;
            var logEvent1 = new LogEventInfo(NLog.LogLevel.Error, "SamlController.MissingEligibility", null, debug, null, null);
            reader.WriteLogMessage(logEvent1);
            return NoSaml(url);
        }

        public ActionResult NoSaml(string url)
        {
            NoSamlModel model = new NoSamlModel();
            model.Url = url;
            return View("NoSaml", model);
        }

        private string FindValue(ComponentPro.Saml2.Attribute attribute)
        {
            if (attribute.Values != null && attribute.Values.Count > 0)
            {
                try
                {
                    return attribute.Values[0].Data as string;
                    // debug += a.Values[0].Type;
                }
                catch
                {

                }
            }
            return "";
        }


        void HandleLogoutRequest(LogoutRequest message, X509Certificate2 x509Certificate, bool isSP)
        {
            // This is the logged in ID.
            string nameId = message.NameId.NameIdentifier;

            // Do something with the ID, like writing a record about the activity of this user.
            // ...
            if ((!isSP && message.Issuer.NameIdentifier.ToLower().Equals(SamlUtility.EdIssuer)) || (isSP && message.Issuer.NameIdentifier.ToLower().Equals(SamlUtility.EdSPIssuer)))
            {
                // Logout locally.
                HttpContext.Session.Clear();
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                #region Create and Send LogoutResponse
                // We need to send back a LogoutResponse to the IdP
                LogoutResponse response = new LogoutResponse();
                response.Status = new Status(SamlPrimaryStatusCode.Success, null);
                response.Issuer = new Issuer(SamlUtility.IssuerUrl);

                // Send the logout response.
                string logoutUrl = SamlUtility.EdlogicIdpUrl;
                if (!string.IsNullOrEmpty(logoutUrl))
                    response.Redirect(Response, logoutUrl, null, x509Certificate.PrivateKey);
                #endregion
            }
            else
            {
                debug += "Incorrect Issue:" + message.Issuer.NameIdentifier;
            }
        }

        void HandleLogoutResponse(LogoutResponse message)
        {
            //SamlTrace.Log(NLog.LogLevel.Info, "Received a Logout Response");
            var url = Url.Action("Index", "Home");
            // Redirect to the default page.
            Response.Redirect(url, false);
        }
        #endregion

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }*/
    }
}