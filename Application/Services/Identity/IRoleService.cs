using Common.Request.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity
{
    public interface IRoleService
    {
        Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest createRoleRequest);
        Task<IResponseWrapper> GetRolesAsync();
        Task<IResponseWrapper> UpdateRoleAsync(UpdateRoleRequest updateRoleRequest);
        Task<IResponseWrapper> GetRoleById(string roleId);
        Task<IResponseWrapper> DeleteRoleAsync(string roleId);
        Task<IResponseWrapper> GetPermissionsAsync(string roleId);
        Task<IResponseWrapper> UpdateRolePermissionsAsync(UpdateRolePermissionsRequest request);
    }
}