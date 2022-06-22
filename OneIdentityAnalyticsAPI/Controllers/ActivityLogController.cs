using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

using Microsoft.Identity.Web.Resource;

using OneIdentityAnalyticsShared.Models;
using OneIdentityAnalyticsShared.Services;

namespace OneIdentityAnalyticsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [RequiredScope("Reports.Embed")]
    [EnableCors("AllowOrigin")]
    public class ActivityLogController : ControllerBase
    {
        private readonly OneIdentityAnalyticsDBService appOwnsDataDBService;

        public ActivityLogController(OneIdentityAnalyticsDBService appOwnsDataDBService)
        {
            this.appOwnsDataDBService = appOwnsDataDBService;
        }

        [HttpPost]
        public ActionResult<ActivityLogEntry> PostActivityLogEntry(ActivityLogEntry activityLogEntry)
        {
            activityLogEntry = this.appOwnsDataDBService.PostActivityLogEntry(activityLogEntry);
            return Ok();
        }
    }
}
