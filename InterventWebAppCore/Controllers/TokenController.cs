using Intervent.DAL;
using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InterventWebApp
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Token([FromForm] TokenModel request)
        {
            if (request.grant_type.Equals("password"))
            {
                var response = await AccountUtility.GetUser(_userManager, request.username, Encoding.ASCII.GetString(Convert.FromBase64String(request.password)).Substring(6), null, true, request.deviceId, true, _appSettings.VerifyDeviceLogin);
                if (response != null)
                {
                    var errMsg = string.Format(Translate.Message("L467"), request.username, "info@myintervent.com");
                    if (response.EmailConfirmed == false)
                    {
                        return Unauthorized("Your account is not active. Please contact us at info@myintervent.com.");
                    }
                    else if (response.User != null)
                    {
                        BearerToken token = GenerateToken(response.User, request.deviceId, request.token);
                        if (token != null)
                            return Ok(token);
                    }
                    else
                    {
                        if (response.error == null)
                            return Unauthorized(errMsg);
                        else
                            return Unauthorized(response.error);
                    }
                }
                return Unauthorized();
            }
            else if (request.grant_type.Equals("refresh_token"))
            {
                if (string.IsNullOrEmpty(request.refresh_token) || request.refresh_token.Length % 4 != 0 || request.refresh_token.Contains(" ") || request.refresh_token.Contains("\t") || request.refresh_token.Contains("\r") || request.refresh_token.Contains("\n"))
                    return BadRequest("Invalid refresh token");
                else
                {
                    try
                    {
                        string refreshToken = Encoding.ASCII.GetString(Convert.FromBase64String(request.refresh_token));
                        ClaimsModel claim = JsonConvert.DeserializeObject<ClaimsModel>(refreshToken);
                        if (claim != null && MobileUtility.ValidateRefreshToken(claim, request.refresh_token))
                        {
                            var response = await AccountUtility.GetUser(_userManager, null, null, claim.userId, null, null);
                            if (response != null)
                            {
                                BearerToken token = GenerateToken(response.User, claim.deviceId, request.token);
                                if (token != null)
                                    return Ok(token);
                            }
                        }
                    }
                    catch { }
                }
                return BadRequest("Invalid refresh token");
            }
            return BadRequest("Unsupported grant type");
        }

        public string? GenerateRefreshToken(ClaimsModel claim)
        {
            string refreshToken = JsonConvert.SerializeObject(claim);
            string key = Convert.ToBase64String(Encoding.ASCII.GetBytes(refreshToken));
            return MobileUtility.SaveRefreshToken(claim, key) ? key : null;
        }

        public BearerToken GenerateToken(UserDto User, string deviceId, string notificationToken)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", User.Id.ToString()),
                new Claim("FullName", User.FirstName + " " + User.LastName),
                new Claim("Module", AccountController.GetGroups(User)),
                new Claim("RoleCode", AccountController.GetRoleCodes(User)),
                new Claim("ExpirationUrl", User.Organization.Url),
                new Claim("TimeZone", !string.IsNullOrEmpty(User.TimeZone) ? User.TimeZone : TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").Id.ToString()),
                new Claim("TimeZoneName", !string.IsNullOrEmpty(User.TimeZoneName) ? Translate.Message(User.TimeZoneName) : "Eastern Standard Time"),
                new Claim("UserName", User.UserName!),
                new Claim("DeviceId", !string.IsNullOrEmpty(deviceId) ? deviceId : ""),
                new Claim("Token", !string.IsNullOrEmpty(notificationToken) ? notificationToken : ""),
                new Claim("SingleSignOn", "false"),
                new Claim("MobileSignOn", "false"),
            };

            int mins = _appSettings.SessionTimeOut + 5;
            DateTime expires = Convert.ToDateTime(DateTime.UtcNow.AddMinutes(mins).ToString("yyyy-MM-dd hh:mm:ss"));
            ClaimsModel claim = new ClaimsModel { deviceId = deviceId, expiresIn = expires, userId = User.Id };

            string refreshToken = GenerateRefreshToken(claim);
            if (string.IsNullOrEmpty(refreshToken))
                return null;

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtSecret));
            var token = new JwtSecurityToken(
                issuer: _appSettings.JwtValidIssuer,
                audience: _appSettings.JwtValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_appSettings.SessionTimeOut),
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return new BearerToken { access_token = new JwtSecurityTokenHandler().WriteToken(token), refresh_token = refreshToken };
        }
    }
}


