using Intervent.DAL;
using Intervent.Web.DTO.AWV;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InterventWebApp
{
    [ApiController]
    [Route("api/[controller]")]
    /// <summary>
    /// AWV Controller
    /// </summary>
    public class AWVController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public AWVController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Get to access the report link
        /// </summary>
        /// <param name="clientId">Client's Unique Identifier</param>
        /// <param name="userId">Patient's Unique Identifier</param>
        /// <param name="date">Current System Date for request validation</param>
        /// <param name="assessmentDate">AWV Assessment Date</param>
        /// <returns>Reports the one time token</returns>      
        [RequireHttps]
        [HttpGet("{clientId?}/{userId?}")]
        public GetTokenResponse GetLinkId(string clientId, string userId, [FromQuery] string date, [FromQuery] string assessmentDate)
        {
            return AWVUtility.GetToken(_userManager, userId, clientId, assessmentDate, _appSettings.EmailUrl);
        }

        /// <summary>
        /// Post for AWV
        /// </summary>
        /// <param name="clientId">Client's Unique Identifier</param>
        /// <param name="userId">Patient's Unique Identifier</param>
        /// <param name="date">Current System Date for request validation</param>
        /// <param name="data">Post data (from request body) with wellness visit information</param>
        /// <returns>Response Status</returns>
        [RequireHttps]
        [HttpPost("{clientId?}/{userId?}")]
        public AnnualWellnessVisitStatus Post(string clientId, string userId, [FromQuery] string date, [FromBody] PostAnualWellnessVisitDto data)
        {
            return AWVUtility.CreateAnualWellnessVisit(_userManager, data, userId, clientId, _appSettings.EmailUrl);
        }
    }
}
