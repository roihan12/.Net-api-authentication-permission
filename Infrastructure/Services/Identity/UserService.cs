using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Request.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }



        public async Task<IResponseWrapper> GetAllUserAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            if (users.Count > 0)
            {
                var mappedUsers = _mapper.Map<List<UserResponse>>(users);
                return await ResponseWrapper<List<UserResponse>>.SuccessAsync(mappedUsers);
            }
            return await ResponseWrapper.FailAsync("No users found.");
        }

        public async Task<IResponseWrapper> GetUserByIdAsync(string userId)
        {
            var userInDb = await _userManager.FindByIdAsync(userId);
            if (userInDb is not null)
            {
                var mappedUser = _mapper.Map<UserResponse>(userInDb);
                return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser);

            }
            return await ResponseWrapper.FailAsync("User not found.");
        }

        public async Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest userRegistrationRequest)
        {
            var userWithSameEmail = await _userManager.FindByEmailAsync(userRegistrationRequest.Email);
            if (userWithSameEmail is not null)
            {
                return await ResponseWrapper.FailAsync("Email is already taken.");
            }

            var userWithSameUserName = await _userManager.FindByNameAsync(userRegistrationRequest.UserName);
            if (userWithSameUserName is not null)
            {
                return await ResponseWrapper.FailAsync("Username is already taken.");
            }

            var newUser = new ApplicationUser
            {
                FirstName = userRegistrationRequest.FirstName,
                LastName = userRegistrationRequest.LastName,
                Email = userRegistrationRequest.Email,
                UserName = userRegistrationRequest.UserName,
                PhoneNumber = userRegistrationRequest.PhoneNumber,
                IsActive = userRegistrationRequest.ActivateUser,
                EmailConfirmed = userRegistrationRequest.AutoConfirmEmail,
            };

            var password = new PasswordHasher<ApplicationUser>();
            newUser.PasswordHash = password.HashPassword(newUser, userRegistrationRequest.Password);

            var identityResult = await _userManager.CreateAsync(newUser);
            if (identityResult.Succeeded)
            {
                // Assign default role to user
                await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);

                return await ResponseWrapper<string>.SuccessAsync("User registered successfully.");
            }

            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescription(identityResult));


        }

        public async Task<IResponseWrapper> UpdatedUserAsync(UpdateUserRequest updateUserRequest)
        {
            var userInDb = await _userManager.FindByIdAsync(updateUserRequest.UserId);

            if (userInDb is not null)
            {
                userInDb.FirstName = updateUserRequest.FirstName;
                userInDb.LastName = updateUserRequest.LastName;
                userInDb.PhoneNumber = updateUserRequest.PhoneNumber;
                var updateResult = await _userManager.UpdateAsync(userInDb);
                if (updateResult.Succeeded)
                {
                    return await ResponseWrapper<string>.SuccessAsync("User updated successfully.");
                }
                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescription(updateResult));
            }

            return await ResponseWrapper.FailAsync("User not found.");
        }

        public async Task<IResponseWrapper> ChangeUserPasswordAsync(ChangeUserPasswordRequest changePasswordRequest)
        {
            var userInDb = await _userManager.FindByIdAsync(changePasswordRequest.UserId);

            if (userInDb is not null)
            {
                var passwordChangeResult = await _userManager.ChangePasswordAsync(userInDb, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);
                if (passwordChangeResult.Succeeded)
                {
                    return await ResponseWrapper<string>.SuccessAsync("Password changed successfully.");
                }
                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescription(passwordChangeResult));
            }
            return await ResponseWrapper.FailAsync("User not found.");
        }

        public async Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest changeUserStatus)
        {
            var userInDb = await _userManager.FindByIdAsync(changeUserStatus.UserId);

            if (userInDb is not null)
            {
                userInDb.IsActive = changeUserStatus.Activate;
                var updateResult = await _userManager.UpdateAsync(userInDb);
                if (updateResult.Succeeded)
                {
                    return await ResponseWrapper<string>.SuccessAsync(changeUserStatus.Activate ? "User activated successfully." : "User deactivated successfully.");
                }
                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescription(updateResult));
            }

            return await ResponseWrapper.FailAsync("User not found.");
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
    }
}
