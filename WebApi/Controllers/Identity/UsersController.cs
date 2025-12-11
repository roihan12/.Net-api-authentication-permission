using Application.Features.Identity.Users.Commands;
using Application.Features.Identity.Users.Queries;
using Common.Authorization;
using Common.Request.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    public class UsersController : MyBaseController<UsersController>
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registration([FromBody] UserRegistrationCommand userRegistration)
        {
            var response = await MediatorSender.Send(new UserRegistrationCommand { UserRegistration = userRegistration.UserRegistration });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("{userId}")]
        [MustHavePermission(AppFeature.Users, AppAction.Read)]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var response = await MediatorSender.Send(new GetUserByIdQuery { UserId = userId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }

        }

        [HttpGet]
        [MustHavePermission(AppFeature.Users, AppAction.Read)]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await MediatorSender.Send(new GetAllUsersQuery());
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
        [MustHavePermission(AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest updateUser)
        {
            var response = await MediatorSender.Send(new UpdateUserCommand { UpdateUserRequest = updateUser });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut]
        [Route("change-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangeUserPasswordRequest changePasswordRequest)
        {
            var response = await MediatorSender.Send(new ChangeUserPasswordCommand { ChangePasswordRequest = changePasswordRequest });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut]
        [Route("change-status")]
        [MustHavePermission(AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> ChangeUserStatus([FromBody] ChangeUserStatusRequest changeUserStatus)
        {
            var response = await MediatorSender.Send(new ChangeUserStatusCommand { ChangeUserStatus = changeUserStatus });
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
        [Route("roles/{userId}")]
        [MustHavePermission(AppFeature.Users, AppAction.Read)]
        public async Task<IActionResult> GetRolesByUserId(string userId)
        {
            var response = await MediatorSender.Send(new GetRolesQuery { UserId = userId });
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
        [Route("roles")]
        [MustHavePermission(AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesRequest updateUserRolesRequest)
        {
            var response = await MediatorSender.Send(new UpdateUserRolesCommand { UpdateUserRolesRequest = updateUserRolesRequest });
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

