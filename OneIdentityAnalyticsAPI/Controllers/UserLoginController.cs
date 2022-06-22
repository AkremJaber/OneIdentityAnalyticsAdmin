using Microsoft.AspNetCore.Mvc;
using OneIdentityAnalyticsShared.Models;
using OneIdentityAnalyticsShared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;
using Microsoft.AspNetCore.Cors;

namespace OneIdentityAnalyticsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiredScope("Reports.Embed")]
    [EnableCors("AllowOrigin")]
    public class UserLoginController : ControllerBase
    {
        private OneIdentityAnalyticsDBService appOwnsDataDBService;

        public UserLoginController(OneIdentityAnalyticsDBService appOwnsDataDBService)
        {
            this.appOwnsDataDBService = appOwnsDataDBService;
        }

        [HttpPost]
        public ActionResult<User> PostUser(User user)
        {
            string authenticatedUser = this.User.FindFirst("preferred_username").Value;
            if (user.LoginId.Equals(authenticatedUser))
            {
                this.appOwnsDataDBService.ProcessUserLogin(user);
                return NoContent();
            }
            else
            {
                return Forbid();
            }
        }
    }
}
