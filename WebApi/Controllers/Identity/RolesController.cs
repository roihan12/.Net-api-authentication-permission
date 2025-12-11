using Application.Features.Identity.Roles.Commands;
using Application.Features.Identity.Roles.Queries;
using Common.Authorization;
using Common.Request.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : MyBaseController<RolesController>
    {
        [HttpPost]
        [MustHavePermission(AppFeature.Roles, AppAction.Create)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest createRoleRequest)
        {
            var response = await MediatorSender.Send(new CreateRoleCommand { CreateRoleRequest = createRoleRequest });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet]
        [MustHavePermission(AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetRoles()
        {
            var response = await MediatorSender.Send(new GetRolesQuery());
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }
        }

        [HttpPut]
        [MustHavePermission(AppFeature.Roles, AppAction.Update)]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest updateRoleRequest)
        {
            var response = await MediatorSender.Send(new UpdateRoleCommand { UpdateRoleRequest = updateRoleRequest });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("{roleId}")]
        [MustHavePermission(AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetRoleById([FromRoute] string roleId)
        {
            var response = await MediatorSender.Send(new GetRoleByIdCommand { RoleId = roleId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }
        }

        [HttpDelete("{roleId}")]
        [MustHavePermission(AppFeature.Roles, AppAction.Delete)]
        public async Task<IActionResult> DeleteRole([FromRoute] string roleId)
        {
            var response = await MediatorSender.Send(new DeleteRoleCommand { RoleId = roleId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("permissions/{roleId}")]
        [MustHavePermission(AppFeature.RolesClaims, AppAction.Read)]
        public async Task<IActionResult> GetPermissions([FromRoute] string roleId)
        {
            var response = await MediatorSender.Send(new GetRolePermissionsQuery { RoleId = roleId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }
        }

        [HttpPut("update-permissions")]
        [MustHavePermission(AppFeature.RolesClaims, AppAction.Update)]
        public async Task<IActionResult> UpdatePermissions([FromBody] UpdateRolePermissionsRequest updateRolePermissionsRequest)
        {
            var response = await MediatorSender.Send(new UpdateRolePermissionsCommand { UpdateRolePermissionsRequest = updateRolePermissionsRequest });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

    }
}
