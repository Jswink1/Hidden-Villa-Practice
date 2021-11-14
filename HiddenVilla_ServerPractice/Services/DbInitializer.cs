using HiddenVilla_Models;
using DataAccess.Data;
using HiddenVilla_ServerPractice.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiddenVilla_ServerPractice.Services
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db,
                             UserManager<IdentityUser> userManager,
                             RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                // If there are any pending migrations
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    // Migrate the DB
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {
                throw;
            }

            // If any roles already exist in the DB
            if (_db.Roles.Any(x => x.Name == SD.Role_Admin))
            {
                return;
            }

            // Create Roles.
            // Because this Initialize method is not async, call GetAwaiter and GetResult so that CreateAsync will work
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

            // Create a User
            _userManager.CreateAsync(new IdentityUser
            {
                UserName = "admin@user.com",
                Email = "admin@user.com",
                EmailConfirmed = true,
            }, "password")
                .GetAwaiter().GetResult();

            // Assign the user the role of admin
            IdentityUser user = _db.Users.FirstOrDefault(u => u.Email == "admin@user.com");
            _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
        }
    }
}
