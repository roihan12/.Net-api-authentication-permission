using Common.Request.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity
{
    public interface IUserService
    {
        Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest userRegistrationRequest);
        Task<IResponseWrapper> GetUserByIdAsync(string userId);
        Task<IResponseWrapper> GetAllUserAsync();
        Task<IResponseWrapper> UpdatedUserAsync(UpdateUserRequest updateUserRequest);
        Task<IResponseWrapper> ChangeUserPasswordAsync(ChangeUserPasswordRequest changePasswordRequest);
        Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest changeUserStatus);
        Task<IResponseWrapper> GetRolesAsync(string userId);
        Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRolesRequest updateUserRolesRequest);
    }
}
