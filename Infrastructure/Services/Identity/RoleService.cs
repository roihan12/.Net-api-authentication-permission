using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Request.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IMapper mapper, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest createRoleRequest)
        {
            var roleExist = await _roleManager.FindByNameAsync(createRoleRequest.RoleName);
            if (roleExist is not null)
            {
                return await ResponseWrapper<string>.FailAsync("Role already exists.");
            }

            var newRole = new ApplicationRole
            {
                Name = createRoleRequest.RoleName,
                Desciption = createRoleRequest.RoleDescription
            };

            var identityResult = await _roleManager.CreateAsync(newRole);
            if (identityResult.Succeeded)
            {
                return await ResponseWrapper<string>.SuccessAsync("Role created successfully.");
            }

            return await ResponseWrapper<string>.FailAsync(GetIdentityResultErrorDescription(identityResult));
        }

        public async Task<IResponseWrapper> DeleteRoleAsync(string roleId)
        {
            var roleInDb = await _roleManager.FindByIdAsync(roleId);
            if (roleInDb is null)
                return await ResponseWrapper<string>.FailAsync("Role not found.");

            if (roleInDb.Name == AppRoles.Admin)
                return await ResponseWrapper<string>.FailAsync("Cannot delete Admin role.");

            var hasUserUsingRole = await _userManager.Users
                .AnyAsync(u => _userManager.IsInRoleAsync(u, roleInDb.Name).Result);

            if (hasUserUsingRole)
                return await ResponseWrapper<string>.FailAsync($"Role: {roleInDb.Name} is currently assigned to a user");

            var identityResult = await _roleManager.DeleteAsync(roleInDb);

            if (identityResult.Succeeded)
                return await ResponseWrapper<string>.SuccessAsync("Role deleted successfully.");

            return await ResponseWrapper<string>.FailAsync(GetIdentityResultErrorDescription(identityResult));
        }

        public async Task<IResponseWrapper> GetPermissionsAsync(string roleId)
        {
            var roleInDb = await _roleManager.FindByIdAsync(roleId);

            if (roleInDb is not null)
            {
                var allPermisssions = AppPermissions.AllPermissions;
                var roleClaimResponse = new RoleClaimResponse
                {
                    Role = new()
                    {
                        Id = roleId,
                        Name = roleInDb.Name,
                        Description = roleInDb.Desciption
                    },
                    RoleClaims = new()
                    {

                    }
                };

                var currentRoleClaims = await GetAllClaimsForRole(roleId);
                var allPermissionsNames = allPermisssions.Select(ap => ap.Name).ToList();
                var currentRoleClaimsValues = currentRoleClaims.Select(crc => crc.ClaimValue).ToList();

                var currentAssignedRoleClaimsPermissions = allPermissionsNames.Intersect(currentRoleClaimsValues).ToList();
                foreach (var permission in allPermisssions)
                {
                    if (currentAssignedRoleClaimsPermissions.Any(carc => carc == permission.Name))
                    {
                        roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel
                        {
                            RoleId = roleId,
                            ClaimType = AppClaim.Permission,
                            ClaimValue = permission.Name,
                            Descriptions = permission.Description,
                            Group = permission.Group,
                            IsAssignedToRole = true
                        });
                    }
                    else
                    {
                        roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel
                        {
                            RoleId = roleId,
                            ClaimType = AppClaim.Permission,
                            ClaimValue = permission.Name,
                            Descriptions = permission.Description,
                            Group = permission.Group,
                            IsAssignedToRole = false
                        });
                    }
                }

                return await ResponseWrapper<RoleClaimResponse>.SuccessAsync(roleClaimResponse);


            }

            return await ResponseWrapper<RoleClaimResponse>.FailAsync("Role not found.");

        }

        public async Task<IResponseWrapper> GetRoleById(string roleId)
        {
            var roleInDb = await _roleManager.FindByIdAsync(roleId);
            if (roleInDb is not null)
            {
                var mappedRole = _mapper.Map<RoleResponse>(roleInDb);
                return await ResponseWrapper<RoleResponse>.SuccessAsync(mappedRole);
            }
            return await ResponseWrapper<string>.FailAsync("Role not found.");
        }

        public async Task<IResponseWrapper> GetRolesAsync()
        {
            var allRoles = await _roleManager.Roles.ToListAsync();
            if (allRoles.Count > 0)
            {
                var mappedRoles = _mapper.Map<List<RoleResponse>>(allRoles);

                return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(mappedRoles);
            }

            return await ResponseWrapper<string>.FailAsync("No roles found.");
        }

        public async Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest updateRoleRequest)
        {
            var isRoleExist = await _roleManager.FindByIdAsync(updateRoleRequest.RoleId);
            if (isRoleExist is not null)
            {

                if (isRoleExist.Name != AppRoles.Admin)
                {
                    isRoleExist.Name = updateRoleRequest.RoleName;
                    isRoleExist.Desciption = updateRoleRequest.RoleDescription;
                    var identityResult = await _roleManager.UpdateAsync(isRoleExist);
                    if (identityResult.Succeeded)
                    {
                        return await ResponseWrapper<string>.SuccessAsync("Role updated successfully.");
                    }
                    return await ResponseWrapper<string>.FailAsync(GetIdentityResultErrorDescription(identityResult));
                }

                return await ResponseWrapper<string>.FailAsync("Cannot update Admin role.");

            }

            return await ResponseWrapper<string>.FailAsync("Role not found or cannot update Admin role.");

        }

        private List<string> GetIdentityResultErrorDescription(IdentityResult identityResult)
        {
            var errorsDescriptions = new List<string>();
            foreach (var error in identityResult.Errors)
            {
                errorsDescriptions.Add(error.Description);
            }
            return errorsDescriptions;
        }

        private async Task<List<RoleClaimViewModel>> GetAllClaimsForRole(string roleId)
        {
            var roleClaims = await _context.RoleClaims
                .Where(rc => rc.RoleId == roleId)
                .ToListAsync();
            if (roleClaims.Count > 0)
            {
                var mappedRoleClaims = _mapper.Map<List<RoleClaimViewModel>>(roleClaims);
                return mappedRoleClaims;
            }

            return new List<RoleClaimViewModel>();

        }

        public async Task<IResponseWrapper> UpdateRolePermissionsAsync(UpdateRolePermissionsRequest request)
        {
            var roleInDb = await _roleManager.FindByIdAsync(request.RoleId);
            if (roleInDb == null)
                return await ResponseWrapper<string>.FailAsync("Role not found.");

            if (roleInDb.Name == AppRoles.Admin)
                return await ResponseWrapper<string>.FailAsync("Cannot update permissions for Admin role.");

            // 🚨 1. VALIDASI PERMISSIONS
            var validPermissionNames = AppPermissions.AllPermissions
                .Select(p => p.Name)
                .ToHashSet();

            var invalidClaims = request.RoleClaims
                .Where(rc => rc.IsAssignedToRole && !validPermissionNames.Contains(rc.ClaimValue))
                .Select(rc => rc.ClaimValue)
                .ToList();

            if (invalidClaims.Any())
            {
                return await ResponseWrapper<string>.FailAsync(
                    $"Invalid permissions detected: {string.Join(", ", invalidClaims)}"
                );
            }

            // Permissions final yang valid
            var permissionsToAssign = request.RoleClaims
                .Where(rc => rc.IsAssignedToRole)
                .ToList();

            // 🚀 2. TRANSACTION untuk Automatic Rollback
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // DELETE semua role claim lama
                var claimsToDelete = _context.RoleClaims
                    .Where(rc => rc.RoleId == roleInDb.Id);

                _context.RoleClaims.RemoveRange(claimsToDelete);

                // INSERT semua claim baru
                var mappedClaims = permissionsToAssign
                    .Select(permission => _mapper.Map<ApplicationRoleClaim>(permission))
                    .ToList();

                await _context.RoleClaims.AddRangeAsync(mappedClaims);

                // Commit 1x
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await ResponseWrapper<string>.SuccessAsync(
                    "Role permissions updated successfully."
                );
            }
            catch (Exception ex)
            {
                // Rollback otomatis
                await transaction.RollbackAsync();
                return await ResponseWrapper<string>.FailAsync($"Unexpected error: {ex.Message}");
            }
        }


        //public async Task<IResponseWrapper> UpdateRolePermissionsAsync(UpdateRolePermissionsRequest request)
        //{
        //    var roleInDb = await _roleManager.FindByIdAsync(request.RoleId);
        //    if (roleInDb is not null)
        //    {
        //        if (roleInDb.Name == AppRoles.Admin)
        //        {
        //            return await ResponseWrapper<string>.FailAsync("Cannot update permissions for Admin role.");
        //        }

        //        var permissionsToBeAssigned = request.RoleClaims.Where(rc => rc.IsAssignedToRole == true)
        //            .ToList();

        //        var currentlyAssignedRoleClaims = await _roleManager.GetClaimsAsync(roleInDb);

        //        // Drop all currently assigned role claims
        //        foreach (var currentRoleClaim in currentlyAssignedRoleClaims)
        //        {
        //            await _roleManager.RemoveClaimAsync(roleInDb, currentRoleClaim);

        //        }

        //        // Assign new permissions to role

        //        foreach (var permission in permissionsToBeAssigned)
        //        {
        //            var mappedRoleClaim = _mapper.Map<ApplicationRoleClaim>(permission);
        //            await _context.RoleClaims.AddAsync(mappedRoleClaim);
        //            await _context.SaveChangesAsync();
        //        }
        //        return await ResponseWrapper<string>.SuccessAsync("Role permissions updated successfully.");

        //    }
        //    return await ResponseWrapper<string>.FailAsync("Role not found.");

        //}
    }
}
