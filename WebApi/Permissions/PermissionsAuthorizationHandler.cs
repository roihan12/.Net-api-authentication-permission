using Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Permissions
{
    public class PermissionsAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        public PermissionsAuthorizationHandler()
        {

        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User is null)
            {
                await Task.CompletedTask;
            }

            var permissions = context.User.Claims
                .Where(c => c.Type == AppClaim.Permission && c.Value == requirement.Permission && c.Issuer == "LOCAL AUTHORITY");
           
            if (permissions.Any())
            {
                context.Succeed(requirement);
                await Task.CompletedTask;
            }
        }
    }
}
