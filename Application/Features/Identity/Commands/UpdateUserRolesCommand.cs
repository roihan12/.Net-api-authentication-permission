using Application.Services.Identity;
using Common.Request.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Commands
{
    public class UpdateUserRolesCommand : IRequest<IResponseWrapper>
    {
        public UpdateUserRolesRequest UpdateUserRolesRequest { get; set; }
    }

    public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, IResponseWrapper>
    {
        private readonly IUserService _userService;
        public UpdateUserRolesCommandHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IResponseWrapper> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            return await _userService.UpdateUserRolesAsync(request.UpdateUserRolesRequest);
        }
    }
}
