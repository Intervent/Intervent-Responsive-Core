using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace InterventWebApp.Controllers
{
    public class ASAOAuthController : Controller
    {
        private readonly AppSettings _appSettings;

        public ASAOAuthController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<ActionResult> ConnecttoASA()
        {
            var userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            var language = "eng";
            if (HttpContext.Session.GetString(SessionContext.ParticipantLanguagePreference) == "fr")
            {
                language = "fra";
            }
            string jwt = createJSONWebToken(language, "respondent", userId.ToString());
            using (HttpClient client = new HttpClient())
            {
                var values = new Dictionary<string, string> { { "token", jwt.ToString() } };
                var request = new FormUrlEncodedContent(values);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsync(_appSettings.ASATokenValidation, request);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return Redirect(_appSettings.ASAUrl + "?token =" + jwt);
                }
            }
            return RedirectToAction("Error", "Account");
        }

        private string createJSONWebToken(String language, String role, String userId)
        {
            var symmetricKey = Encoding.ASCII.GetBytes(_appSettings.ASASecret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsdata = new[]
            {
                new Claim("language", language),
                new Claim("user", "ITest"+userId),
                new Claim("version" , "20"),
                new Claim("roles", "[\"" + role + "\"]"),
                new Claim("study", _appSettings.ASAStudyID),
                new Claim("redirect", _appSettings.EmailUrl),
                new Claim("studyAccess", "[\"" + _appSettings.ASAStudyAbbreviation + "\"]")
            };
            var SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha512);
            var token = new JwtSecurityToken(
                issuer: "Intervent",
                expires: DateTime.Now.AddMinutes(90),
                claims: claimsdata,
                signingCredentials: SigningCredentials
            );
            var jwt = tokenHandler.WriteToken(token);
            return jwt;
        }
    }
}