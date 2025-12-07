using Common.Authorization;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class ApplicationDbSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public ApplicationDbSeeder(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task SeedDatabaseAsync()
        {
            // Check for pending and apply if any
            await CheckAndApplyPendingMigrationAsync();

            // Seed roles
            await SeedRolesAsync();

            // Seed basicUser
            await SeedUserBasicAsync();

            // Seed user(Admin)
            await SeedUserAdminAsync();





        }

        private async Task CheckAndApplyPendingMigrationAsync()
        {
            if (_dbContext.Database.GetPendingMigrations().Any())
            {
                await _dbContext.Database.MigrateAsync();
            }

        }








        private async Task SeedUserAdminAsync()
        {
            string adminUserName = AppCredentials.Email[..AppCredentials.Email.IndexOf('@')].ToLowerInvariant();

            var adminUser = new ApplicationUser
            {
                FirstName = "System",
                LastName = "Administrator",
                UserName = adminUserName,
                Email = AppCredentials.Email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true,
                NormalizedEmail = AppCredentials.Email.ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),

            };

            if (!await _userManager.Users.AnyAsync(u => u.Email == AppCredentials.Email))
            {
                var password = new PasswordHasher<ApplicationUser>();
                adminUser.PasswordHash = password.HashPassword(adminUser, AppCredentials.Password);
                await _userManager.CreateAsync(adminUser);
            }

            if (!await _userManager.IsInRoleAsync(adminUser, AppRoles.Basic) && !await _userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
            {
                await _userManager.AddToRolesAsync(adminUser, AppRoles.DefaultRoles);
            }

        }


        private async Task SeedUserBasicAsync()
        {
         

            var basicUser = new ApplicationUser
            {
                FirstName = "Basic",
                LastName = "User",
                UserName = "basicuser",
                Email = "basicuser@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true,
                NormalizedEmail = "BASICUSER@GMAIL.COM",
                NormalizedUserName = "BASICUSER"

            };

            if (!await _userManager.Users.AnyAsync(u => u.Email == "basicuser@gmail.com"))
            {
                var password = new PasswordHasher<ApplicationUser>();
                basicUser.PasswordHash = password.HashPassword(basicUser, AppCredentials.Password);
                await _userManager.CreateAsync(basicUser);
            }

            if (!await _userManager.IsInRoleAsync(basicUser, AppRoles.Basic))
            {
                await _userManager.AddToRoleAsync(basicUser, AppRoles.Basic);
            }

        }

        private async Task SeedRolesAsync()
        {
            foreach (var roleName in AppRoles.DefaultRoles)
            {
                if (await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName)
                    is not ApplicationRole role)
                {
                    role = new ApplicationRole
                    {
                        Name = roleName,
                        Desciption = $"{roleName} Role."
                    };

                    await _roleManager.CreateAsync(role);
                }

                // Assign Permissions to Roles
                if (roleName == AppRoles.Admin)
                {
                    await AssignPermissionsToRoleAsync(role, AppPermissions.AdminPermissions);
                }
                else if (roleName == AppRoles.Basic)
                {
                    await AssignPermissionsToRoleAsync(role, AppPermissions.BasicPermissions);
                }
            }
        }

        private async Task AssignPermissionsToRoleAsync(ApplicationRole role, IReadOnlyList<AppPermission> permissions)
        {
            var currentClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var permission in permissions) { 
                if (!currentClaims.Any(claim => claim.Type == AppClaim.Permission && claim.Value == permission.Name))
                {
                    await _dbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                    {
                        RoleId = role.Id,
                        ClaimType = AppClaim.Permission,
                        ClaimValue = permission.Name,
                        Descriptions = permission.Description,
                        Group = permission.Group,
                    });

                    await _dbContext.SaveChangesAsync();
                }

            }
        }



    }
}
