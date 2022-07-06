using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneIdentityAnalyticsShared.Models;
using OneIdentityAnalyticsShared.Services;
using OneIdentityAnalytics.Models;
using OneIdentityAnalytics.Services;

namespace OneIdentityAnalytics.Controllers
{

    
    public class HomeController : Controller
    {

        private PowerBiServiceApi powerBiServiceApi;
        private OneIdentityAnalyticsDBService AppOwnsDataDBService;

        public HomeController(PowerBiServiceApi powerBiServiceApi, OneIdentityAnalyticsDBService AppOwnsDataDBService)
        {
            this.powerBiServiceApi = powerBiServiceApi;
            this.AppOwnsDataDBService = AppOwnsDataDBService;
        }

        //[AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        //[Authorize(Roles ="Admin")]
        public IActionResult Tenants()
        {
            var model = AppOwnsDataDBService.GetTenants();
            return View(model);
        }

        public IActionResult Tenant(string Name)
        {
            var model = AppOwnsDataDBService.GetTenant(Name);
            var modelWithDetails = powerBiServiceApi.GetTenantDetails(model);
            return View(modelWithDetails);
        }

        public class OnboardTenantModel
        {
            public string TenantName { get; set; }
            public string UID_CCCTenants { get; set; }
            public string XObjectKey { get; set; }

            public string SuggestedDatabase { get; set; }
            public List<SelectListItem> DatabaseOptions { get; set; }
            public string SuggestedAppIdentity { get; set; }
            public List<SelectListItem> AppIdentityOptions { get; set; }
        }

        public IActionResult OnboardTenant()
        {

            var model = new OnboardTenantModel
            {
                TenantName = this.AppOwnsDataDBService.GetNextTenantName(),
                SuggestedDatabase = "SEACDEV01",
                DatabaseOptions = new List<SelectListItem> {
          new SelectListItem{ Text="AcmeCorpSales", Value="AcmeCorpSales" },
          new SelectListItem{ Text="ContosoSales", Value="ContosoSales" },
          new SelectListItem{ Text="MegaCorpSales", Value="MegaCorpSales" }
        }
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult OnboardTenant(string TenantName,string DatabaseServer, string DatabaseName, string DatabaseUserName, string DatabaseUserPassword)
        {
            var ccc = System.Guid.NewGuid().ToString();
            var xobj = "<Key><T>CCCTenants</T><P>"+ccc+"</P></Key>";
            var tenant = new PowerBiTenant
            {
                CCC_Name = TenantName,
                UID_CCCTenants=ccc,
                XObjectKey=xobj,
                CCC_DatabaseServer = DatabaseServer,
                CCC_DatabaseName = DatabaseName,
                CCC_DatabaseUserName = DatabaseUserName,
                CCC_DatabaseUserPassword = DatabaseUserPassword,
            };

            tenant = this.powerBiServiceApi.OnboardNewTenant(tenant);
            this.AppOwnsDataDBService.OnboardNewTenant(tenant);

            return RedirectToAction("Tenants");

        }

        public IActionResult DeleteTenant(string TenantName)
        {
            var tenant = this.AppOwnsDataDBService.GetTenant(TenantName);
            this.powerBiServiceApi.DeleteWorkspace(tenant);
            this.AppOwnsDataDBService.DeleteTenant(tenant);
            return RedirectToAction("Tenants");
        }

        public IActionResult Embed(string AppIdentity, string ReportId, string Tenant)
        {
            var ReporttId = new Guid(ReportId);
            var viewModel = this.powerBiServiceApi.GetReportEmbeddingData(AppIdentity, ReporttId,Tenant).Result;
            return View(viewModel);
        }
        public IActionResult EmbedDash(string dashboardid, string workspaceId, string Tenant)
        {
            var dashboarddid = new Guid(dashboardid);
            //var workkspaceId = new Guid(workspaceId);
            var viewModel = this.powerBiServiceApi.GetDashboardEmbeddingData(dashboarddid,Tenant).Result;
            return View(viewModel);
        }


        public IActionResult Users()
        {
            var model = AppOwnsDataDBService.GetUsers();
            return View(model);
        }
        public IActionResult Persons()
        {
            var model = AppOwnsDataDBService.GetPersons();
            return View(model);
        }
        public IActionResult PersonInAERole()
        {
            var model = AppOwnsDataDBService.GetPersonInAERole();
            return View(model);
        }
        public IActionResult GetUser(string LoginId)
        {
            var model = AppOwnsDataDBService.GetUser(LoginId);
            return View(model);
        }

        public class EditUserModel
        {
            public User User { get; set; }
            public List<SelectListItem> TenantOptions { get; set; }
        }

        public IActionResult EditUser(string LoginId)
        {
            var model = new EditUserModel
            {
                User = AppOwnsDataDBService.GetUser(LoginId),
                TenantOptions = this.AppOwnsDataDBService.GetTenants().Select(tenant => new SelectListItem
                {
                    Text = tenant.CCC_Name,
                    Value = tenant.CCC_Name
                }).ToList(),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            var model = AppOwnsDataDBService.UpdateUser(user);
            return RedirectToAction("Users");
        }



        public class CreateUserModel
        {
            public List<SelectListItem> TenantOptions { get; set; }
        }

        public IActionResult CreateUser()
        {
            var model = new CreateUserModel
            {
                TenantOptions = this.AppOwnsDataDBService.GetTenants().Select(tenant => new SelectListItem
                {
                    Text = tenant.CCC_Name,
                    Value = tenant.CCC_Name
                }).ToList(),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            var model = AppOwnsDataDBService.CreateUser(user);
            return RedirectToAction("Users");
        }

        public IActionResult DeleteUser(string LoginId)
        {
            var user = this.AppOwnsDataDBService.GetUser(LoginId);
            this.AppOwnsDataDBService.DeleteUser(user);
            return RedirectToAction("Users");
        }
 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
