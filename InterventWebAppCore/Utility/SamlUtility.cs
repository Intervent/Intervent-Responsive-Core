namespace InterventWebApp
{
    public class SamlUtility
    {
        /*public static string IssuerUrl = ConfigurationManager.AppSettings["IssuerUrl"];
        public static string RedirectUrl = ConfigurationManager.AppSettings["RedirectUrl"];
        public static string EdlogicIdpUrl = ConfigurationManager.AppSettings["EdlogicIdpUrl"];
        public static string EdlogiSpUrl = ConfigurationManager.AppSettings["EdlogiSpUrl"];
        public static string EdIssuer = ConfigurationManager.AppSettings["EdIssuer"];
        public static string EdSPIssuer = ConfigurationManager.AppSettings["EdSPIssuer"];
        public const string BindingVarName = "binding";
        public const string Binding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";

        #region Service Provider

        public void SamlSPRedirectToIdp(HttpResponseBase Response)
        {
            AuthnRequest authnRequest = BuildAuthenticationRequest();

            //// Create and cache the relay state so we remember which SP resource the user wishes 
            //// to access after SSO.
            //string spResourceUrl = Util.GetAbsoluteUrl(context, FormsAuthentication.GetRedirectUrl("", false));
            string relayState = Guid.NewGuid().ToString();
            //SamlSettings.CacheProvider.Insert(relayState, spResourceUrl, new TimeSpan(1, 0, 0));

            // Send the authentication request to the identity provider over the selected binding.
            string idpUrl = string.Format("{0}?{1}={2}", EdlogicIdpUrl, BindingVarName, System.Net.WebUtility.UrlEncode(Binding));


            authnRequest.SendHttpPost(Response, idpUrl, relayState);

            // Don't send this form.
            Response.End();
        }

        /// <summary>
        /// Builds an authentication request.
        /// </summary>
        /// <returns>The authentication request.</returns>
        private AuthnRequest BuildAuthenticationRequest()
        {
            // Construct the assertion Consumer Service Url.
            string assertionConsumerServiceUrl = string.Format("{0}?{1}={2}", RedirectUrl,
                BindingVarName, WebUtility.UrlEncode(Binding));

            // Create the authentication request.
            AuthnRequest authnRequest = new AuthnRequest();
            authnRequest.Destination = EdlogicIdpUrl;
            authnRequest.Issuer = new Issuer(IssuerUrl);
            authnRequest.ForceAuthn = false;
            authnRequest.NameIdPolicy = new NameIdPolicy("urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress", null, true);
            authnRequest.ProtocolBinding = Binding;
            authnRequest.AssertionConsumerServiceUrl = assertionConsumerServiceUrl;


            // Get the certificate
            X509Certificate2 x509Certificate = (X509Certificate2)HttpContext.Current.Application[Constants.InterventCertificate];

            // Sign the authentication request.
            authnRequest.Sign(x509Certificate);

            return authnRequest;

        }

        //Core recursion function
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }

        /// <summary>
        /// Receives the SAML response from the identity provider.
        /// </summary>
        /// <param name="samlResponse">The SAML Response object.</param>
        /// <param name="relayState">The relay state object.</param>
        public string ExtractResponse(SSOProviderDto provider, out ComponentPro.Saml2.Response samlResponse, out string relayState)
        {
            // Determine the identity provider to service provider binding type.
            // We use a query string parameter rather than having separate endpoints per binding.
            string bindingType = HttpContext.Current.Request.QueryString[BindingVarName];
            string debug = "Extractresponse start" + bindingType;
            samlResponse = null;
            relayState = null;
            //LogReader reader = new LogReader();
            switch (bindingType)
            {
                case SamlBindingUri.HttpPost:
                default:
                    try
                    {
                        var xml = ComponentPro.Saml2.Response.CreateXml(HttpContext.Current.Request, out relayState);
                        //reader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "SamlController", null, xml.InnerXml, null, "Saml Started"));


                        if (xml != null)
                        {

                            foreach (XmlNode child in xml.ChildNodes)
                            {
                                if (child.Name.Contains("Assertion"))
                                {
                                    foreach (XmlNode child1 in child.ChildNodes)
                                    {
                                        if (child1.Name.Contains("AttributeStatement"))
                                        {
                                            foreach (XmlNode child2 in child1.ChildNodes)
                                            {
                                                foreach (XmlNode child3 in child2.ChildNodes)
                                                {
                                                    if (string.IsNullOrEmpty(child3.InnerText))
                                                    {
                                                        child1.RemoveChild(child2);
                                                        debug += "removed";
                                                        break;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                            samlResponse = new Response(xml);
                            debug += "issuer:" + samlResponse.Issuer.NameIdentifier;
                        }
                        else
                        {
                            debug += "empty xml";
                        }
                        //samlResponse = ComponentPro.Saml2.Response.Create(HttpContext.Current.Request);
                    }
                    catch (Exception exception)
                    {
                        debug += exception.Message + exception.StackTrace;
                        return debug;
                    }
                    if (samlResponse != null)
                        debug += "Response is not empty";
                    else
                        debug += "Empty response";
                    relayState = samlResponse.RelayState;
                    break;

            }
            X509Certificate2 x509Certificate = null;
            //string idpKey = provider.
            if (provider == null)
            {
                x509Certificate = (X509Certificate2)HttpContext.Current.Application[Constants.EdlogicCertificate];
            }
            else
            {
                var data = Convert.FromBase64String(provider.Certificate);
                x509Certificate = new X509Certificate2(data);
            }
            debug += samlResponse.SignatureElement.InnerText;


            return debug;
        }
        #endregion

        #region Identity Provider
        /// <summary>
        /// Sends the SAML response to the Service Provider.
        /// </summary>
        /// <param name="context">The HTTP Context.</param>
        /// <param name="samlResponse">The SAML response object.</param>
        /// <param name="ssoState">The authentication state.</param>
        public static void SendResponse(HttpContextBase context, ComponentPro.Saml2.Response samlResponse, SsoAuthnState ssoState)
        {
            // Sign the SAML response 
            X509Certificate2 x509Certificate = HttpContext.Current.Application[Constants.InterventCertificate] as X509Certificate2;

            samlResponse.Sign(x509Certificate);

            string relayState = null;
            if (ssoState != null)
            {
                relayState = ssoState.RelayState;
            }
            // Send the SAML response to the service provider.
            samlResponse.SendHttpPost(context.Response.OutputStream, EdlogiSpUrl, relayState);
        }

        /// <summary>
        /// Builds the SAML response.
        /// </summary>
        /// <param name="context">The HTTP Context object.</param>
        /// <returns>A SAML Response object.</returns>
        public static ComponentPro.Saml2.Response BuildResponse(HttpContextBase context)
        {
            ComponentPro.Saml2.Response samlResponse = new ComponentPro.Saml2.Response();
            string issuerUrl = IssuerUrl;

            samlResponse.Issuer = new Issuer(issuerUrl);

            if (context.User.Identity.IsAuthenticated)
            {
                samlResponse.Status = new Status(SamlPrimaryStatusCode.Success, null);

                Assertion samlAssertion = new Assertion();

                // Use the local user's local identity.
                Subject subject = new Subject(new NameId(HttpContext.Current.Session[SessionContext.ParticipantEmail].ToString()));
                SubjectConfirmation subjectConfirmation = new SubjectConfirmation(SamlSubjectConfirmationMethod.Bearer);
                SubjectConfirmationData subjectConfirmationData = new SubjectConfirmationData();
                subjectConfirmationData.Recipient = EdlogiSpUrl;
                subjectConfirmation.SubjectConfirmationData = subjectConfirmationData;
                subject.SubjectConfirmations.Add(subjectConfirmation);
                samlAssertion.Subject = subject;

                // Create a new authentication statement.
                AuthnStatement authnStatement = new AuthnStatement();
                authnStatement.AuthnContext = new AuthnContext();
                authnStatement.AuthnContext.AuthnContextClassRef = new AuthnContextClassRef(SamlAuthenticationContext.Unspecified);
                samlAssertion.Statements.Add(authnStatement);



                // If you need to add custom attributes, uncomment the following code
                // #region Custom Attributes
                AttributeStatement attributeStatement = new AttributeStatement();
                attributeStatement.Attributes.Add(new ComponentPro.Saml2.Attribute("email", SamlAttributeNameFormat.Basic, "email",
                HttpContext.Current.Session[SessionContext.ParticipantEmail].ToString()));
                var fullName = HttpContext.Current.Session[SessionContext.ParticipantName].ToString().Split(' ');
                attributeStatement.Attributes.Add(new ComponentPro.Saml2.Attribute("givenName", SamlAttributeNameFormat.Basic, "givenName", fullName[0]));
                attributeStatement.Attributes.Add(new ComponentPro.Saml2.Attribute("surName", SamlAttributeNameFormat.Basic, "surName", fullName[1]));
                if (HttpContext.Current.Session[SessionContext.Gender] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[SessionContext.Gender].ToString()))
                {
                    byte gender = byte.Parse(HttpContext.Current.Session[SessionContext.Gender].ToString());
                    string sGender;
                    if (gender == 1)
                        sGender = "MALE";
                    else
                        sGender = "FEMALE";
                    attributeStatement.Attributes.Add(new ComponentPro.Saml2.Attribute("gender", SamlAttributeNameFormat.Basic, "Gender", sGender));
                }
                if (HttpContext.Current.Session[SessionContext.DOB] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[SessionContext.DOB].ToString()))
                {
                    DateTime DOB = DateTime.Parse(HttpContext.Current.Session[SessionContext.DOB].ToString());
                    attributeStatement.Attributes.Add(new ComponentPro.Saml2.Attribute("birth_date", SamlAttributeNameFormat.Basic, "Birthdate", DOB.ToString("MM/dd/yyyy")));
                }
                string phone = GetPhoneNumber();
                if (!string.IsNullOrEmpty(phone))
                {
                    attributeStatement.Attributes.Add(new ComponentPro.Saml2.Attribute("phone", SamlAttributeNameFormat.Basic, "phone", phone));
                }
                if (HttpContext.Current.Session[SessionContext.Zip] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[SessionContext.Zip].ToString()))
                {
                    string zip = HttpContext.Current.Session[SessionContext.Zip].ToString();
                    attributeStatement.Attributes.Add(new ComponentPro.Saml2.Attribute("postal_code", SamlAttributeNameFormat.Basic, "Postal Code", zip));
                }

                // Insert a custom token key to the SAML response.
                //attributeStatement.Attributes.Add(new ComponentPro.Saml2.Attribute("CustomTokenForVerification", SamlAttributeNameFormat.Basic, null,
                //"YourEncryptedTokenHere"));

                samlAssertion.Statements.Add(attributeStatement);
                samlResponse.Assertions.Add(samlAssertion);
                // #endregion

            }
            else
            {
                samlResponse.Status = new Status(SamlPrimaryStatusCode.Responder, SamlSecondaryStatusCode.AuthnFailed, "The user is not authenticated at the identity provider");
            }

            return samlResponse;
        }

        private static string GetPhoneNumber()
        {
            int userId;
            if (HttpContext.Current.Session[SessionContext.ParticipantId] != null)
                userId = Convert.ToInt32(HttpContext.Current.Session[SessionContext.ParticipantId].ToString());
            else
                userId = Convert.ToInt32(HttpContext.Current.Session[SessionContext.UserId].ToString());
            Intervent.Web.DataLayer.AccountReader reader = new Intervent.Web.DataLayer.AccountReader();
            var user = reader.GetUserById(userId);

            if (user.ContactMode.HasValue)
            {
                string phoneNumber = "";
                if (user.ContactMode.Value == 1)
                {
                    phoneNumber = user.HomeNumber;
                }
                else if (user.ContactMode.Value == 2)
                {
                    phoneNumber = user.WorkNumber;
                }
                else if (user.ContactMode.Value == 3)
                {
                    phoneNumber = user.CellNumber;
                }
                phoneNumber = System.Text.RegularExpressions.Regex.Replace(phoneNumber, "[^0-9.]", "");
                if (phoneNumber.Length == 10)
                    return phoneNumber;
            }

            return null;
        }

        /// <summary>
        /// Processes the authentication request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="authnRequest">The AuthnRequest object.</param>
        /// <param name="relayState">The relayState string.</param>
        public static string ProcessAuthnRequest(HttpContextBase context, out AuthnRequest authnRequest, out string relayState)
        {
            // Use a single endpoint and use a query string parameter to determine the Service Provider to Identity Provider binding type.
            string bindingType = context.Request.QueryString["binding"];
            string debug = string.Empty;
            switch (bindingType)
            {
                case SamlBindingUri.HttpRedirect:
                    X509Certificate2 x509Certificate = (X509Certificate2)context.Application[Constants.EdlogicCertificate];

                    authnRequest = AuthnRequest.CreateFromHttpRedirect(context.Request.RawUrl, x509Certificate.PublicKey.Key);
                    relayState = authnRequest.RelayState;
                    return debug;

                case SamlBindingUri.HttpPost:
                    authnRequest = AuthnRequest.CreateFromHttpPost(context.Request);
                    relayState = authnRequest.RelayState;
                    break;
                default:
                    authnRequest = AuthnRequest.CreateFromHttpPost(context.Request);
                    relayState = authnRequest.RelayState;
                    break;

            }

            if (authnRequest != null && authnRequest.IsSigned())
            {
                // Get the loaded certificate.
                X509Certificate2 x509Certificate = (X509Certificate2)context.Application[Constants.EdlogicCertificate];
                debug += authnRequest.Issuer.NameIdentifier;
                // And validate the authentication request with the certificate.
                if (!authnRequest.Validate(x509Certificate))
                {
                    debug += "The authentication request signature failed to verify.";
                }
            }
            return debug;
        }

        #endregion

        #region Helper

        public bool CanCreateUserProfile(SSOProviderDto provider, string uniqueId, out EligibilityDto eligibilityDto)
        {
            eligibilityDto = null;
            if (provider != null && provider.HasEligibility)
            {
                PortalReader reader = new PortalReader();
                ListPortalsRequest request = new ListPortalsRequest();
                request.organizationId = provider.OrganizationId;
                request.onlyActive = true;
                var portals = reader.ListPortals(request).portals;
                if (portals != null && portals.FirstOrDefault() != null)
                {
                    ParticipantReader pReader = new ParticipantReader();
                    GetEligibilityRequest eRequest = new GetEligibilityRequest();
                    List<int> portalIds = new List<int>();
                    portalIds.Add(portals.FirstOrDefault().Id);
                    eRequest.PortalIds = portalIds;
                    eRequest.UniqueId = uniqueId;
                    var eligibility = pReader.GetEligibility(eRequest);
                    if (eligibility != null && eligibility.Eligibility != null)
                    {
                        eligibilityDto = eligibility.Eligibility;
                        return true;
                    }
                }
                return false;
            }
            return true;
        }

        public string GetType(AttributeType type, SSOProviderDto provider)
        {
            if (provider != null)
            {
                return provider.FindNameByType(type).ToLower();
            }
            else
            {
                return type.ToString().ToLower();
            }
        }

        public string[] GetPossibleValues(AttributeType type, SSOProviderDto provider)
        {
            string[] value = null;
            if (provider != null)
            {
                value = provider.FindPossibleValue(type);
            }

            if (value == null)
            {
                if (type == AttributeType.Gender)
                {
                    value = new string[]{
                        "male",
                        "female"
                    };
                }
            }

            return value;
        }


        public SSOProviderDto GetProviderByName(string name)
        {
            var list = PopulateSSOProvider();
            if (list != null)
            {
                var provider = list.FirstOrDefault(p => p.ProviderName.ToUpper() == name.ToUpper());
                return provider;
            }
            return null;
        }

        private IList<SSOProviderDto> PopulateSSOProvider()
        {
            var ssoproviders = HttpContext.Current.Cache["SSO"] as IList<SSOProviderDto>;
            if (ssoproviders == null)
            {
                SSOReader reader = new SSOReader();
                ssoproviders = reader.GetAllProviders();
                HttpContext.Current.Cache.Add("SSO", ssoproviders, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
            return ssoproviders;
        }
        #endregion
    }

    #region Auth state

    public class SsoAuthnState
    {
        AuthnRequest _authnRequest;
        public AuthnRequest AuthnRequest
        {
            get { return _authnRequest; }
            set { _authnRequest = value; }
        }

        string _relayState;
        public string RelayState
        {
            get { return _relayState; }
            set { _relayState = value; }
        }

        SamlBinding _idpProtocolBinding;
        public SamlBinding IdpProtocolBinding
        {
            get { return _idpProtocolBinding; }
            set { _idpProtocolBinding = value; }
        }

        string _assertionConsumerServiceUrl;
        public string AssertionConsumerServiceURL
        {
            get { return _assertionConsumerServiceUrl; }
            set { _assertionConsumerServiceUrl = value; }
        }
    }
    #endregion*/
    }
}