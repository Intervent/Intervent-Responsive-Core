using Microsoft.AspNetCore.Mvc;

namespace InterventWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemController : ControllerBase
    {
        [HttpGet("{caller?}")]

        // GET: api/System
        public HealthStatus GetStatus(string caller)
        {
            HealthStatus status = new HealthStatus();
            status.Status = true;
            return status;
        }
    }

    public class HealthStatus
    {
        public bool Status { get; set; }
    }
}