using Common.Responses.Identity;

namespace Common.Request.Identity
{
    public class UpdateRolePermissionsRequest
    {
        public string RoleId { get; set; }
        public List<RoleClaimViewModel> RoleClaims { get; set; }
    }
}
