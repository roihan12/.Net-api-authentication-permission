using Application.Services.Identity;
using Common.Request.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands
{
    public class UpdateUserCommand : IRequest<IResponseWrapper>
    {
        public UpdateUserRequest UpdateUserRequest { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResponseWrapper>
    {
        private readonly IUserService _userService;
        public UpdateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IResponseWrapper> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.UpdatedUserAsync(request.UpdateUserRequest);
        }
    }



}
