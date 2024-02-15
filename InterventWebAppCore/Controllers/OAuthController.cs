using Intervent.DAL;
using Intervent.HWS;
using Intervent.Utils;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TimeZoneConverter;

namespace InterventWebApp.Controllers
{
    public class OAuthController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly IHostEnvironment _environment;
        private readonly ApplicationUserManager _userManager;

        public OAuthController(ApplicationUserManager userManager, IOptions<AppSettings> appSettings, IHostEnvironment environment)
        {
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _environment = environment;
        }

        [HttpGet]
        public ActionResult Intuity(IntuityRedirectModel model)
        {
            byte[] bytes = Convert.FromBase64String(model.state);
            string originalText = Encoding.ASCII.GetString(bytes);
            var jo = JObject.Parse(originalText);
            // var state = model.user_auth;
            var stateStr = jo["user_auth"].ToString();
            var nonce = Guid.NewGuid().ToString("N");

            var url = $"{_appSettings.IntuityOAuthURL}/oauth/authorize?" +
                // "&acr_values=" + HttpUtility.HtmlEncode(AcrValues) +
                "client_id=" + _appSettings.IntuityClientId +
                "&redirect_uri=" + $"{_appSettings.EmailUrl}/OAuth/Callback" +
                "&response_type=code" +
                "&scope=openid+email+profile" +
                "&state=" + model.state;
            //+ "&nonce=" + nonce;
            try
            {
                LogReader reader = new LogReader();
                var logEvent1 = new LogEventInfo(NLog.LogLevel.Info, "SamlController", null, stateStr + "~" + url, null, null);
                reader.WriteLogMessage(logEvent1);
            }
            catch { }
            return Redirect(url);
        }

        public async Task<ActionResult> Callback()
        {
            LogReader reader = new LogReader();
            try
            {
                // Retrieve code and state from query string, pring for debugging
                var code = HttpContext.Request.Query["code"];
                var state = HttpContext.Request.Query["state"];

                var logEvent1 = new LogEventInfo(NLog.LogLevel.Info, "OAuthController", null, code + "~" + state, null, null);
                reader.WriteLogMessage(logEvent1);
                var handler = new JwtSecurityTokenHandler();

                // Send GET to make token request
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();

                    data["code"] = code;
                    data["grant_type"] = "authorization_code";
                    data["Authorization"] = "Basic " + AccountUtility.Base64Encode(_appSettings.IntuityClientId + ":" + _appSettings.IntuityClientIssuer);
                    var baseUrl = _appSettings.EmailUrl;
                    if (!baseUrl.EndsWith("/"))
                    {
                        baseUrl = baseUrl + "/";
                    }
                    data["redirect_uri"] = $"{baseUrl}OAuth/Callback";
                    //data["redirect_uri"] = $"http://146.88.110.217/InterventWebApp/OAuth/Callback";

                    wb.Headers.Add("Authorization", "Basic " + AccountUtility.Base64Encode(_appSettings.IntuityClientId + ":" + _appSettings.IntuityClientIssuer));
                    var response = wb.UploadValues($"{_appSettings.IntuityOAuthURL}/oauth/token", "POST", data);
                    //var responseString = "{'access_token':'22253e36d860896e9eb82cefa14d76072e75d79c501f5f542da506cd01933a6d','token_type':'Bearer','expires_in':7200,'scope':'openid email profile','created_at':1561598520,'id_token':'eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IllvdXpxa1dwLWxWaHpELWRlXzFPNWRHVXdScG9reXpDcFh2dmVOdk9mOVEifQ.eyJpc3MiOiJJbnR1aXR5IFBvZ29QYXR0ZXJucyIsInN1YiI6IjQ4ZjgyZjliLWJlYjItNDFmZi05NjJjLWMyMDk2ZTU3ZjI2MSIsImF1ZCI6IkRkT2VoYmxXc01ORjJwbWpQYkRaWE40dVlvSzMyb0dJUG9mbk02cG9Qa1Rnd3ciLCJleHAiOjE1NjE1OTg2NDAsImlhdCI6MTU2MTU5ODUyMCwiYXV0aF90aW1lIjoxNTYxNTk4NDkzfQ.CPSH4kt0CasV4tEl4kX73qUaI930xx0jS2jN3XO4Ku3wtfAGz7A-2kggQgbshNCO183xCkXD8PPMN1ifKtU6UcbkZ4v1Cc78EvOzBAztVpTldB-WIdfrnZS22JPOrR0u3hudkgYDznJEyNrS_CN-pkcgCW57EGMioDUKeA0ASQcviC_mXeMh9Jt9gUyeAZK8DFqQ7qCiNRV2YfM0vleJHYTVKRUUVq-ShTn4228I7-RFhvVpwKodsD4LzIPU3UFPhT_D-ooQ7QCoMuydNjhJ-h-7ovf3LUJWUmwlQIdShzXcev9k2fpt0rx6N2kU2r6cTStbfuknP-wzlCpxroMpMc3HfhEL0KkwekwabKZYs8n5oDw7eu5VSatvXXXNkPNF53xFYtsyAu2GOywX7cSci0ipUMRSCvPxnnOX8LYvEPJQMiY7oKi3eQp4ZfQsp0FOqeMYFytht_4Wh7_GxLyMR1BhgjjpX5paT81ZLBN5Wlw5G1TXR2sKew7Sl7mepwzn0A9YBHadoK-ciE6E_N0EltuaMZicdEiNQD0AUG6W7ZD0yHzy4egQA4014rlmNzP0OZdV4zeMkdDsWRK41htxxIOgj6FHNPaXFTHqWCE-MW4xwEdqS9sx5oj0sxlgcX6WArTp_pg4PXd0hikTTYcxw-wsxplU34NZT6_sGkvHm2E'}";

                    var responseString = Encoding.ASCII.GetString(response);
                    logEvent1 = new LogEventInfo(NLog.LogLevel.Info, "OAuthController", null, responseString, null, null);
                    reader.WriteLogMessage(logEvent1);
                    //responseString = "{'access_token': '811d06fdd71fdf9fcd478a3b5364e799d761d87f19d81c6c53f5f496cd00bba0','token_type': 'Bearer','expires_in': 7200,'scope': 'email profile openid','created_at': 1554806707,'id_token': 'eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik5DcDZqdk1LbEN5dWFaQVg0cS1NY0NzVHE1d09QQWdyU2FFQkhvZlEtWmMifQ.eyJpc3MiOiJJbnR1aXR5IFBvZ29QYXR0ZXJucyIsInN1YiI6IjRiYzBhMzQxLTkyZWQtNDhkOS05YzMwLTk5NmI5YjNmMTZjOSIsImF1ZCI6IkRkT2VoYmxXc01ORjJwbWpQYkRaWE40dVlvSzMyb0dJUG9mbk02cG9Qa1Rnd3ciLCJleHAiOjE1NTQ4MDY4MjcsImlhdCI6MTU1NDgwNjcwNywiYXV0aF90aW1lIjoxNTU0ODA2MjUwfQ.utIn_rK7McFP9JnBDj0OX1QRpPWuykfAJJdJAE1NRYpoQTb4-6ly0DNgHUZj4IQZ4S0f9pSMx6Srm_EUxJwarmHNJe6SL2OEjq1T80V3nc-mV3xYtYwrtpaXjx0lH7n5vDgiDi4BXC-MSPp-hmoMyiDOXRbPJlP-x2746ygo1JG22e5ZqFHflRsqc8Pd9z7Xr5xhee4OBVDgbNZGhUdyUSN0rVHNbFocLjXa54p1HYHo4tGlEdR1zC486s-FI0ovqXfV8yV7tFPp2o4JzQoEzLDSrqg_ssp0xvAbHxrpSv7kt7Skw_DTJ2-54P5YJ8OTCu8MmmmMqQ9rKNiE-UZU4NURYPvSWDTvVeHVRoM8v6LjerXFGdBmuhJdgAe23OVYn4PNdq7BzcwTJ0aOZetNnIafEYWEXCWTuB5WIRmkYRxxkrgy1TyLFD_c-jWJ7sImgr88wW9WUEQyCiekVisHM5Z5MmY_NMKwEOJDcK03mMbgHahIJeai0JWfkXfp9cBeU4ps_9UNQkhRDCBXWJl7kWOj9-pIs6qRTSsZAB5kPoGC0M_-myFAcjargmnH96o4_igqM8wovwi6_flC0y3PRVJxasJBD0XkFJh2x6pPXIQK7JJGaBnxPWZK-s9xCn87BMAtW37SwxOMv1xzOGwb3AeuQNEScUfQjBF4jGlLwKM'}";
                    dynamic tokenResponse = JObject.Parse(responseString);

                    var token = ValidateToken((String)tokenResponse.id_token) as JwtSecurityToken;
                    var userToken = token.Claims.First(c => c.Type == "sub").Value;

                    var logEvent = new LogEventInfo(NLog.LogLevel.Info, "OAuthController", null, userToken, null, null);
                    reader.WriteLogMessage(logEvent);
                    //
                    var userresponse = await AccountUtility.GetValidUserByToken(_userManager, userToken);
                    var userDto = userresponse != null ? userresponse.User : null;
                    if (userDto != null)
                    {
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        var identity = new ClaimsIdentity("InterventOauthLogin");
                        HttpContext.Session.SetInt32(SessionContext.UserId, userDto.Id);
                        HttpContext.Session.SetString(SessionContext.TermsSSO, userDto.Organization.TermsForSSO.ToString());
                        HttpContext.Session.SetString(SessionContext.TermsAccepted, userDto.TermsAccepted.ToString());
                        identity.AddClaim(new Claim("UserId", userDto.Id.ToString()));
                        identity.AddClaim(new Claim("FullName", userDto.FirstName + " " + userDto.LastName));
                        identity.AddClaim(new Claim("Module", AccountController.GetGroups(userDto)));
                        identity.AddClaim(new Claim("RoleCode", AccountController.GetRoleCodes(userDto)));
                        identity.AddClaim(new Claim("ExpirationUrl", userDto.Organization.Url));
                        identity.AddClaim(new Claim("SingleSignOn", "true"));
                        identity.AddClaim(new Claim("MobileSignOn", "false"));
                        if (userDto.TimeZoneId.HasValue)
                            identity.AddClaim(new Claim("TimeZone", CommonUtility.GetTimeZones(userDto.TimeZoneId).TimeZones[0].TimeZoneId));
                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        logEvent = new LogEventInfo(NLog.LogLevel.Info, userDto.Id.ToString(), null, "Intuity User Login", null, null);
                        reader.WriteLogMessage(logEvent);

                        //return RedirectToAction("Terms", "Participant");
                        AccountUtility.LogUserLastAccess(userDto.Id, true, null);

                        return RedirectToAction("Stream", "Participant");
                    }
                }
            }
            catch (Exception ex)
            {
                var debug = "Exception:" + ex.Message + ex.InnerException;
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "OAuthController", null, debug, null, ex);
                reader.WriteLogMessage(logEvent);
            }
            return RedirectToAction("NotAuthorized", "Account");

        }

        static RSACryptoServiceProvider DecodeX509PublicKey(byte[] x509Key)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            using (var mem = new MemoryStream(x509Key))
            {
                using (var binr = new BinaryReader(mem))    //wrap Memory Stream with BinaryReader for easy reading
                {
                    try
                    {
                        var twobytes = binr.ReadUInt16();
                        switch (twobytes)
                        {
                            case 0x8130:
                                binr.ReadByte();    //advance 1 byte
                                break;
                            case 0x8230:
                                binr.ReadInt16();   //advance 2 bytes
                                break;
                            default:
                                return null;
                        }

                        var seq = binr.ReadBytes(15);


                        twobytes = binr.ReadUInt16();
                        if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                            binr.ReadByte();    //advance 1 byte
                        else if (twobytes == 0x8203)
                            binr.ReadInt16();   //advance 2 bytes
                        else
                            return null;

                        var bt = binr.ReadByte();
                        if (bt != 0x00)     //expect null byte next
                            return null;

                        twobytes = binr.ReadUInt16();
                        if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                            binr.ReadByte();    //advance 1 byte
                        else if (twobytes == 0x8230)
                            binr.ReadInt16();   //advance 2 bytes
                        else
                            return null;

                        twobytes = binr.ReadUInt16();
                        byte lowbyte = 0x00;
                        byte highbyte = 0x00;

                        if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                            lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                        else if (twobytes == 0x8202)
                        {
                            highbyte = binr.ReadByte(); //advance 2 bytes
                            lowbyte = binr.ReadByte();
                        }
                        else
                            return null;
                        byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                        int modsize = BitConverter.ToInt32(modint, 0);

                        byte firstbyte = binr.ReadByte();
                        binr.BaseStream.Seek(-1, SeekOrigin.Current);

                        if (firstbyte == 0x00)
                        {   //if first byte (highest order) of modulus is zero, don't include it
                            binr.ReadByte();    //skip this null byte
                            modsize -= 1;   //reduce modulus buffer size by 1
                        }

                        byte[] modulus = binr.ReadBytes(modsize); //read the modulus bytes

                        if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                            return null;
                        int expbytes = binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                        byte[] exponent = binr.ReadBytes(expbytes);

                        // We don't really need to print anything but if we insist to...
                        //showBytes("\nExponent", exponent);
                        //showBytes("\nModulus", modulus);

                        // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                        RSAParameters rsaKeyInfo = new RSAParameters
                        {
                            Modulus = modulus,
                            Exponent = exponent
                        };
                        rsa.ImportParameters(rsaKeyInfo);
                        return rsa;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
        }

        private SecurityToken ValidateToken(string jwtToken)
        {
            string certKey = "MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA5E2ED1kQ59jQik9eTwi69t18DQ3bJfKyJupAfIs/SXD0tPUL3LDdeCnYKnIc6LVIrJaksisbQ5FbHjhJC+l6zME+GdvXebjY5BYJVcksi17ZYCYJ0YS5vR0vEhg+sw9zj3ZzxlUIn0k5ubDCKncH/uDB49HtNd/4EInS8h0tgFVXLzlHLQmW8tg7PyUz3VP7/GhlmvvV9Balokrswv1GNgb2V8chjWrqAQNMw0UN8Xscz4wHSuHEYjfAlY8W98QdUNebPj1hyigZgEHr+oGdGijrB8gIA8Yn0RrTLMwEUx7OOiNd3xdkz9iRf9tEvpuM3kuQ2HdSwrHDN1FI1ulwaT6e1QsTN8vhACP+KkJOfaLXpObpBG9jvmj9+rIHdgjmXpWIQ3NVJU/NRMEgtRWpN5GBVq/3qJlVuDrp5XNzkPXncNareU8FeRfL1Pp5Aea4HnyI2MnD1D390riynr0XraOevtxEnU6n2El6TwjoNGhSqnhYMPwh2gQT+xz84YmHxskDFJbYABRfKBm0QEQ9BMLJrBrUdWrVWchjY8CCruMKTbH5u7hnN7jy5YdvMRRe7MYbeAO4CQLAP8+KmkylPJESYBUYIw9nvdsdpi5UNd94HXOCh+6ma+1SLBNT7vIeXlERrTNS1394GMXbblboAy0GyKRx0MnuxR5eQ/exK4UCAwEAAQ==";
            SecurityToken validatedToken;
            if (_appSettings.RequiresHttps)
            {
                certKey = "MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAwDRHWFJUplsZePTX/CAfNawcTGuS+SS3ldPKgsmA2EDatcL0frpI1DzRaUf+QTwQ8QdVyLwpHfvfHFKmfnQdvBiUlY64jGl09JsDe+VzjB58gNahHJ0xgj+ETDpMQjfQ62gZmUe+BUvoCUb4UHvMXLy3ey1kbkiI/E/weEecL+fjGl8dKJq3LMp9OUfHfYcssE2cseaBzUXOyps5KURSSHqJLD+BVEqzXe7DB5RLajAw+8nTcGhVTApuqr/mCMa+9GBsjmnNkyNtynFPuyndkSx9J0nHCzrBbcS4G0LC1SIVbeTLxh6Pm9pg9loLMGQXlsjv0Od7bp94rT8ssqaEfzgws3zPqDkJ2BWdeU/NVMGb8321mBF/8VSZJUWOLpmsog9zjwtMV9KLjmmGFcwLcHw4R7+im9Jav9BY/fEgUabeuH86jbMsLuIBrBXCtdEE09rgaXAvLoVEZWJBAcSDpZv5021euAosOxBNssgp7urR2/5+T+Xl9d04ozCaMHTbYKgVKVOpgoEkSi1Qz6raVjuNtsEn7vqjshXEtgN9vJkCwA7+Uso/2XnF4klNtHT049r7aYsUnNWu3KM0d8pwNONO397Arr+mFuTiGGgsl0RAPIxwD8Wkyk0WELTWXywqswYk+fjgMs0r86rZ+faVWr7LUV1onokBlvpvDZf2vNUCAwEAAQ==";
            }
            try
            {
                var data = Convert.FromBase64String(certKey);
                RSACryptoServiceProvider rsa = DecodeX509PublicKey(data);

                var handler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = false, // Because there is no expiration in the generated token
                    ValidateAudience = false, // Because there is no audiance in the generated token
                    ValidateIssuer = true,
                    ValidIssuer = "Intuity PogoPatterns",
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                };
                //handler.ValidateToken("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IllvdXpxa1dwLWxWaHpELWRlXzFPNWRHVXdScG9reXpDcFh2dmVOdk9mOVEifQ.eyJpc3MiOiJJbnR1aXR5IFBvZ29QYXR0ZXJucyIsInN1YiI6IjQ4ZjgyZjliLWJlYjItNDFmZi05NjJjLWMyMDk2ZTU3ZjI2MSIsImF1ZCI6IkRkT2VoYmxXc01ORjJwbWpQYkRaWE40dVlvSzMyb0dJUG9mbk02cG9Qa1Rnd3ciLCJleHAiOjE1NjE1OTg2NDAsImlhdCI6MTU2MTU5ODUyMCwiYXV0aF90aW1lIjoxNTYxNTk4NDkzfQ.CPSH4kt0CasV4tEl4kX73qUaI930xx0jS2jN3XO4Ku3wtfAGz7A-2kggQgbshNCO183xCkXD8PPMN1ifKtU6UcbkZ4v1Cc78EvOzBAztVpTldB-WIdfrnZS22JPOrR0u3hudkgYDznJEyNrS_CN-pkcgCW57EGMioDUKeA0ASQcviC_mXeMh9Jt9gUyeAZK8DFqQ7qCiNRV2YfM0vleJHYTVKRUUVq-ShTn4228I7-RFhvVpwKodsD4LzIPU3UFPhT_D-ooQ7QCoMuydNjhJ-h-7ovf3LUJWUmwlQIdShzXcev9k2fpt0rx6N2kU2r6cTStbfuknP-wzlCpxroMpMc3HfhEL0KkwekwabKZYs8n5oDw7eu5VSatvXXXNkPNF53xFYtsyAu2GOywX7cSci0ipUMRSCvPxnnOX8LYvEPJQMiY7oKi3eQp4ZfQsp0FOqeMYFytht_4Wh7_GxLyMR1BhgjjpX5paT81ZLBN5Wlw5G1TXR2sKew7Sl7mepwzn0A9YBHadoK-ciE6E_N0EltuaMZicdEiNQD0AUG6W7ZD0yHzy4egQA4014rlmNzP0OZdV4zeMkdDsWRK41htxxIOgj6FHNPaXFTHqWCE-MW4xwEdqS9sx5oj0sxlgcX6WArTp_pg4PXd0hikTTYcxw-wsxplU34NZT6_sGkvHm2E", validationParameters, out validatedToken);

                handler.ValidateToken(jwtToken, validationParameters, out validatedToken);

            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                var debug = "Exception:" + ex.Message + ex.InnerException;
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "OAuthController", null, debug, null, ex);
                reader.WriteLogMessage(logEvent);
                throw;
            }
            return validatedToken;

        }

        [HttpGet]
        public ActionResult AuthorizeMediOrbis()
        {
            LogReader reader = new LogReader();
            try
            {
                var ua = HttpContext.Request.Query["ua"];
                if (!string.IsNullOrEmpty(ua))
                {
                    byte[] bytes = Convert.FromBase64String(ua);
                    string originalText = Encoding.ASCII.GetString(bytes);
                    var jo = JObject.Parse(originalText);
                    var refId = jo["ref"].ToString();
                    var user_auth = jo["user_auth"].ToString();

                    var url = $"{_appSettings.MediOrbisUrl}msmd-patient/getOauthCode?" +
                        "client_id=" + _appSettings.MediOrbisClientId +
                        "&redirect_uri=" + $"{_appSettings.EmailUrl}/OAuth/MediOrbisCallback" +
                        "&response_type=code" +
                        "&scope=openid+email+profile" +
                        "&state=" + refId;

                    reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Info, "OAuthController.AuthorizeMediOrbis", null, "Step-1 : " + refId + "~" + user_auth + "~" + url, null, null));
                    return Redirect(url);
                }
                else
                {
                    reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "OAuthController.AuthorizeMediOrbis", null, "BadRequest", null, null));
                }
            }
            catch (Exception e)
            {
                reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "OAuthController.AuthorizeMediOrbis", null, e.Message, null, e));
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        public async Task<ActionResult> MediOrbisCallback()
        {
            LogReader reader = new LogReader();
            CommonReader commonReader = new CommonReader();
            try
            {
                var code = HttpContext.Request.Query["code"];
                var state = HttpContext.Request.Query["state"];

                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(state))
                {
                    reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Info, "OAuthController.MediOrbisCallback", null, "Step-2 : " + code + "~" + state, null, null));

                    MediOrbisOAuth oAuth = null;
                    using (var wb = new WebClient())
                    {
                        string token = AccountUtility.Base64Encode(_appSettings.MediOrbisClientId + _appSettings.MediOrbisClientSecret);
                        using (var client = new HttpClient(new LogRequestAndResponseHandler(new HttpClientHandler())))
                        {
                            client.BaseAddress = new Uri(_appSettings.MediOrbisUrl);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                            var content = new FormUrlEncodedContent(new[]
                            {
                                new KeyValuePair<string, string>("code", code),
                                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                                new KeyValuePair<string, string>("redirect_uri", $"{_appSettings.EmailUrl}/OAuth/MediOrbisCallback"),
                            });

                            var response = await client.PostAsync("/msmd-patient/getOauthScopeData", content);
                            if (response.IsSuccessStatusCode)
                            {
                                var apiJsonResponse = response.Content.ReadAsStringAsync().Result;
                                reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Info, "OAuthController.MediOrbisCallback", null, "Step-3 : " + apiJsonResponse, null, null));
                                oAuth = JsonConvert.DeserializeObject<MediOrbisOAuth>(apiJsonResponse);
                            }
                            else
                            {
                                reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "OAuthController.MediOrbisCallback", null, "OAuth token response : " + response.StatusCode, null, null));
                            }
                        }
                    }

                    if (oAuth != null)
                    {
                        JwtSecurityToken token = ValidateMediOrbisToken(oAuth.id_token);
                        if (token != null)
                        {
                            var userToken = token.Claims.First(c => c.Type == "sub").Value;
                            var timeStamp = double.Parse(token.Claims.First(c => c.Type == "exp").Value);
                            DateTime expireDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            expireDate = expireDate.AddSeconds(timeStamp);

                            if (expireDate > DateTime.UtcNow)
                            {
                                reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Info, "OAuthController.MediOrbisCallback", null, "Step-4 : " + userToken + "~" + expireDate, null, null));
                                MediOrbisProfile userProfile = JsonConvert.DeserializeObject<MediOrbisProfile>(userToken);

                                OrganizationDto organizationDto = PortalUtility.GetOrganizationByCode(userProfile.clientRefId);
                                if (organizationDto != null)
                                {
                                    UserDto user = null;
                                    int orgId = organizationDto.Id.Value;

                                    var regResponse = AccountUtility.CheckifRegistered(userProfile.refId, orgId);
                                    if (regResponse.recordExist == true)
                                    {
                                        var userResponse = await AccountUtility.GetUser(_userManager, regResponse.User.UserName, null, null, false, null);
                                        if (userResponse != null && userResponse.User != null)
                                            user = userResponse.User;
                                    }
                                    else
                                    {
                                        CreateUserModel request = new CreateUserModel();
                                        UserDto createUser = new UserDto();
                                        if (string.IsNullOrEmpty(userProfile.email))
                                            userProfile.email = userProfile.refId + orgId + "@mediorbisnoemail.com";
                                        createUser.Email = userProfile.email;
                                        // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                                        createUser.PasswordHash = CommonUtility.GeneratePassword(10, 3);
                                        createUser.FirstName = userProfile.firstname;
                                        createUser.MiddleName = userProfile.middlename;
                                        createUser.LastName = userProfile.lastname;
                                        createUser.UserName = userProfile.email;
                                        createUser.UniqueId = userProfile.refId;
                                        if (!string.IsNullOrEmpty(userProfile.dob))
                                            createUser.DOB = DateTime.Parse(userProfile.dob);
                                        if (!string.IsNullOrEmpty(userProfile.gender))
                                            createUser.Gender = string.Equals(userProfile.gender.ToLower(), GenderDto.Male.Description.ToLower()) ? GenderDto.Male.Key : GenderDto.Female.Key;
                                        createUser.City = userProfile.city;
                                        createUser.Address = userProfile.address;
                                        if (!string.IsNullOrEmpty(userProfile.state))
                                        {
                                            var stateResponse = CommonUtility.GetState(userProfile.state);
                                            if (stateResponse != null && stateResponse.state != null)
                                                createUser.State = stateResponse.state.Id;
                                        }
                                        if (!string.IsNullOrEmpty(userProfile.country))
                                        {
                                            var countryResponse = CommonUtility.GetCountry(userProfile.country);
                                            if (countryResponse != null && countryResponse.country != null)
                                                createUser.Country = countryResponse.country.Id;
                                        }
                                        createUser.Zip = userProfile.postalcode;
                                        createUser.HomeNumber = userProfile.phone;
                                        if (!string.IsNullOrEmpty(userProfile.language))
                                        {
                                            var languageResponse = CommonUtility.GetLanguages().Languages.Where(x => x.Language == userProfile.language).FirstOrDefault();
                                            if (languageResponse != null)
                                                createUser.LanguagePreference = languageResponse.LanguageCode;
                                        }
                                        if (!string.IsNullOrEmpty(userProfile.unit))
                                        {
                                            var unitResponse = ListOptions.GetUnits().Where(x => x.Text == userProfile.unit).FirstOrDefault();
                                            if (unitResponse != null)
                                                createUser.Unit = Convert.ToByte(unitResponse.Value);
                                        }
                                        if (!string.IsNullOrEmpty(userProfile.timezone))
                                        {
                                            string timezoneId = TZConvert.RailsToWindows(userProfile.timezone.Substring(userProfile.timezone.IndexOf(")") + 2));
                                            createUser.TimeZoneId = commonReader.GetTimeZoneByTimeZoneId(timezoneId);
                                        }
                                        createUser.OrganizationId = orgId;
                                        createUser.EmailConfirmed = false;
                                        createUser.TermsAccepted = true;
                                        request.user = createUser;
                                        request.rootPath = _environment.ContentRootPath;
                                        request.InfoEmail = _appSettings.InfoEmail;
                                        request.SecureEmail = _appSettings.SecureEmail;
                                        request.SMPTAddress = _appSettings.SMPTAddress;
                                        request.PortNumber = _appSettings.PortNumber;
                                        request.SecureEmailPassword = _appSettings.SecureEmailPassword;
                                        request.MailAttachmentPath = _appSettings.MailAttachmentPath;
                                        var User = await AccountUtility.CreateUser(_userManager, request);
                                        var userResponse = await AccountUtility.GetUser(_userManager, userProfile.email, null, null, false, null);
                                        if (userResponse != null && userResponse.User != null)
                                        {
                                            user = userResponse.User;
                                            var response = MediOrbis.SendUserProgramStatus(new MediOrbisRequest { refId = userProfile.refId, selfCareTool = userProfile.selfCareTool, status = (int)MediOrbisProgramStatus.NotStarted }, _appSettings.MediOrbisUrl, _appSettings.MediOrbisSecretKey);
                                            if (response.Status && response.StatusCode == HttpStatusCode.OK)
                                                reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Info, "MediOrbis Program Status", null, "Program status update for user : " + user.Id + " - RefId : " + userProfile.refId, null, null));
                                            else
                                                reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "MediOrbis Program Status", null, "Update program status failed for user : " + user.Id + " - RefId : " + userProfile.refId + " - Status code : " + response.StatusCode + " - Message : " + response.ErrorMsg, null, null));
                                        }
                                        else
                                            reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "OAuthController.MediOrbisCallback", null, "Error while create user : " + userResponse.error, null, null));
                                    }

                                    if (user != null)
                                    {
                                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                                        var identity = new ClaimsIdentity("InterventOauthLogin");
                                        HttpContext.Session.SetInt32(SessionContext.UserId, user.Id);
                                        HttpContext.Session.SetString(SessionContext.TermsSSO, user.Organization.TermsForSSO.ToString());
                                        //HttpContext.Session.SetString(SessionContext.TermsAccepted, user.Organization.TermsAccepted.ToString());
                                        identity.AddClaim(new Claim("UserId", user.Id.ToString()));
                                        identity.AddClaim(new Claim("FullName", user.FirstName + " " + user.LastName));
                                        identity.AddClaim(new Claim("Module", AccountController.GetGroups(user)));
                                        identity.AddClaim(new Claim("RoleCode", AccountController.GetRoleCodes(user)));
                                        identity.AddClaim(new Claim("ExpirationUrl", user.Organization.Url));
                                        identity.AddClaim(new Claim("SingleSignOn", "true"));
                                        identity.AddClaim(new Claim("MobileSignOn", "false"));
                                        if (user.TimeZoneId.HasValue)
                                            identity.AddClaim(new Claim("TimeZone", CommonUtility.GetTimeZones(user.TimeZoneId).TimeZones[0].TimeZoneId));
                                        HttpContext.Session.SetString(SessionContext.LandingPage, user.Organization.Url);
                                        HttpContext.Session.SetString(SessionContext.ProgramCode, userProfile.selfCareTool);
                                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                                        reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Info, user.Id.ToString(), null, "SSO User Login", null, null));
                                        return RedirectToAction("Stream", "Participant");
                                    }
                                }
                                else
                                {
                                    reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "OAuthController.MediOrbisCallback", null, "Org code not found. " + userToken, null, null));
                                }
                            }
                            else
                            {
                                reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "OAuthController.MediOrbisCallback", null, "Expired token", null, null));
                            }
                        }
                        else
                        {
                            reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "OAuthController.MediOrbisCallback", null, "Invalid token", null, null));
                        }
                    }
                }
                else
                {
                    reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "OAuthController.MediOrbisCallback", null, "Bad Request", null, null));
                }
            }
            catch (Exception ex)
            {
                reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "OAuthController.MediOrbisCallback", null, "Exception : " + ex.Message + ex.InnerException, null, ex));
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        private static JwtSecurityToken ValidateMediOrbisToken(string jwtToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(jwtToken);
                return jsonToken as JwtSecurityToken;
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                var debug = "Exception:" + ex.Message + ex.InnerException;
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "OAuthController.ValidateMediOrbisToken", null, debug, null, ex);
                reader.WriteLogMessage(logEvent);
            }
            return null;
        }
    }
}