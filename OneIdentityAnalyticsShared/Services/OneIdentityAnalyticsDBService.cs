 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OneIdentityAnalyticsShared.Models;
        

namespace OneIdentityAnalyticsShared.Services
{

    public class OneIdentityAnalyticsDBService
        {

        private readonly OneIdentityAnalyticsDB dbContext;
         
        public OneIdentityAnalyticsDBService(OneIdentityAnalyticsDB dbContext)
        {
            this.dbContext = dbContext;
        }

        public string GetNextTenantName()
        {
            var appNames = dbContext.CCCTenants.Select(tenant => tenant.CCC_Name).ToList();
            string baseName = "Tenant";
            string nextName;
            int counter = 0;
            do
            {
                counter += 1;
                nextName = baseName + counter.ToString("00");
            }
            while (appNames.Contains(nextName));
            return nextName;
        }

        public void OnboardNewTenant(PowerBiTenant tenant)
        {
            dbContext.CCCTenants.Add(tenant);
            dbContext.SaveChanges();
        }

        public IList<PowerBiTenant> GetTenants()
        {
            return dbContext.CCCTenants
                   .Select(tenant => tenant)
                   .OrderBy(tenant => tenant.CCC_Name)
                   .ToList();
        }

        public PowerBiTenant GetTenant(string TenantName)
        {
            var tenant = dbContext.CCCTenants.Where(tenant => tenant.CCC_Name == TenantName).FirstOrDefault();
            return tenant;
        }

        public void DeleteTenant(PowerBiTenant tenant)
        {

            // unassign any users in the tenant
            var tenantUsers = dbContext.Users.Where(user => user.TenantName == tenant.CCC_Name);
            foreach (var user in tenantUsers)
            {
                user.TenantName = "";
                dbContext.Users.Update(user);
            }
            dbContext.SaveChanges();

            // delete the tenant
            dbContext.CCCTenants.Remove(tenant);
            dbContext.SaveChanges();
            return;
        }

        public IList<User> GetUsers()
        {
            return dbContext.Users
                   .Select(user => user)
                   .OrderByDescending(user => user.LastLogin)
                   .ToList();
        }

        public User GetUser(string LoginId)
        {
            var user = dbContext.Users.Where(user => user.LoginId == LoginId).First();
            return user;
        }
     
        public IList<Person> GetPersons()
        {
            return dbContext.Person
                   .Select(person => person)
                  // .OrderByDescending(person => user.LastLogin)
                   .ToList();
        }
        public IList<PersonInAERole> GetPersonInAERole()
        {
            return dbContext.PersonInAERole
                   .Select(personinae => personinae)
                   // .OrderByDescending(person => user.LastLogin)
                   .ToList();
        }

        public User UpdateUser(User currentUser)
        {
            var users = dbContext.Users.Where(user => user.LoginId == currentUser.LoginId);
            User user;
            if (users.Count() > 0)
            {
                user = users.First();
            }
            else
            {
                user = new User();
            }
          //  user.UserName = currentUser.UserName;
            user.CanEdit = currentUser.CanEdit;
            user.CanCreate = currentUser.CanCreate;
            user.TenantName = currentUser.TenantName;
            dbContext.SaveChanges();
            return user;
        }

        public User CreateUser(User newUser)
        {
            var users = dbContext.Users.Where(user => user.LoginId == newUser.LoginId);
            User user;
            if (users.Count() > 0)
            {
                user = users.First();
            }
            else
            {
                user = new User();
                user.Created = DateTime.Now;
            }
            user.LoginId = newUser.LoginId;
          //  user.UserName = !string.IsNullOrEmpty(newUser.UserName) ? newUser.UserName : user.UserName;
            user.TenantName = !string.IsNullOrEmpty(newUser.TenantName) ? newUser.TenantName : user.TenantName;
            user.CanEdit = newUser.CanEdit;
            user.CanCreate = newUser.CanCreate;
            user.LastLogin = DateTime.Now;
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            return user;
        }

        public void DeleteUser(User user)
        {
            dbContext.Users.Remove(user);
            dbContext.SaveChanges();
            return;
        }

        public async Task<List<ActivityLogEntry>> GetActivityLog()
        {
            return await this.dbContext.ActivityLog.ToListAsync();
        }

        public async Task<ActivityLogEntry> GetActivityLogEntry(int id)
        {
            return await this.dbContext.ActivityLog.FindAsync(id);
        }

        public ActivityLogEntry PostActivityLogEntry(ActivityLogEntry activityLogEntry)
        {
            activityLogEntry.Created = DateTime.Now;
            activityLogEntry.WorkspaceId = (dbContext.CCCTenants.Where(tenant => tenant.CCC_Name == activityLogEntry.Tenant).First()).CCC_WorkspaceId;
            dbContext.ActivityLog.Add(activityLogEntry);
            dbContext.SaveChanges();
            return activityLogEntry;
        }

        public void ProcessUserLogin(User currentUser)
        {

            bool userExists = this.dbContext.Users.Any(user => user.LoginId == currentUser.LoginId);

            if (userExists)
            {
                currentUser = dbContext.Users.Find(currentUser.LoginId);
                currentUser.LastLogin = DateTime.Now;
                dbContext.SaveChanges();
            }
            else
            {
                currentUser.Created = DateTime.Now;
                currentUser.LastLogin = DateTime.Now;
                currentUser.CanEdit = false;
                currentUser.CanCreate = false;
                dbContext.Users.Add(currentUser);
                dbContext.SaveChanges();
            }

        }
    }

}
