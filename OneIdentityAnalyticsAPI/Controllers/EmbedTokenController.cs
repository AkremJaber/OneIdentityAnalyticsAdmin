using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using OneIdentityAnalyticsAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneIdentityAnalyticsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiredScope("Reports.Embed")]
    [EnableCors("AllowOrigin")]
    public class EmbedTokenController : ControllerBase
    {
        private PowerBiServiceApi powerBiServiceApi;

        public EmbedTokenController(PowerBiServiceApi powerBiServiceApi)
        {
            this.powerBiServiceApi = powerBiServiceApi;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            string user = this.User.FindFirst("preferred_username").Value;
            return await this.powerBiServiceApi.GetEmbedToken(user);
        }
    }
}
