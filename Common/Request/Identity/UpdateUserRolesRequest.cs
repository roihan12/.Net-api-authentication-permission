using Common.Responses.Identity;

namespace Common.Request.Identity
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; }
        public List<UserRoleViewModel> Roles { get; set; }
    }
}
