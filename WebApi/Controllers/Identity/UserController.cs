using Application.Features.Identity.Commands;
using Application.Features.Identity.Queries;
using Common.Request.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    public class UserController : MyBaseController<UserController>
    {
        [HttpPost]
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

    }
}

