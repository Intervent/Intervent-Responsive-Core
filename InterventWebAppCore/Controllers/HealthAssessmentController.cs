using Intervent.Business;
using Intervent.DAL;
using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InterventWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthAssessmentController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public HealthAssessmentController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        // GET: HealthAssessment
        [RequireHttps]
        [HttpGet("{clientId?}/{userId?}")]
        public IActionResult GetCategories(string clientId, string userId, [FromQuery] string date)
        {
            HraManager hraManager = new HraManager(_userManager, _appSettings.DTCOrgCode);
            string UserId = userId.Substring(3);
            // var UserId = Request.Headers.GetValues("UserId").FirstOrDefault();
            var categories = hraManager.GetHRAAssessment(UserId, _appSettings.SalesForceOrgId);
            return Ok(categories);
        }

        [RequireHttps]
        [HttpGet("{clientId?}/{userId?}")]
        public IActionResult GetProfile(string clientId, string userId, [FromQuery] string date)
        {
            HraManager hraManager = new HraManager(_userManager, _appSettings.DTCOrgCode);
            string UserId = null;
            if (userId.Length > 3)
                UserId = userId.Substring(3);
            var categories = hraManager.GetUserProfile(UserId, _appSettings.SalesForceOrgId);
            return Ok(categories);
        }

        [RequireHttps]
        public IActionResult SaveProfile(string clientId, string userId, [FromQuery] string date, [FromBody] APISaveUserProfileRequest data)
        {
            HraManager hraManager = new HraManager(_userManager, _appSettings.DTCOrgCode);
            string UserId = null;
            if (userId.Length > 3)
                UserId = userId.Substring(3);
            return Ok(hraManager.SaveUserProfile(data, _appSettings.SalesForceOrgId));
        }

        [RequireHttps]
        public IActionResult SaveHRA(string clientId, string userId, [FromQuery] string date, [FromBody] APISaveHRAQuestionRequest data)
        {
            HraManager hraManager = new HraManager(_userManager, _appSettings.DTCOrgCode);
            string UserId = userId.Substring(3);
            return Ok(hraManager.SaveHRAQuestion(data, _appSettings.SalesForceOrgId));
        }
    }
}
