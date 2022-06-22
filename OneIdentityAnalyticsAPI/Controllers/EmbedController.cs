using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using OneIdentityAnalyticsAPI.Models;
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
    public class EmbedController : ControllerBase
    {
        private PowerBiServiceApi powerBiServiceApi;

        public EmbedController(PowerBiServiceApi powerBiServiceApi)
        {
            this.powerBiServiceApi = powerBiServiceApi;
        }

        [HttpGet]
        public async Task<EmbeddedViewModel> Get()
        {
            string user = this.User.FindFirst("preferred_username").Value;
            return await this.powerBiServiceApi.GetEmbeddedViewModel(user);
        }
    }
}
