using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Queries
{
    public class GetRoleByIdCommand : IRequest<IResponseWrapper>
    {
        public string RoleId { get; set; }
    }

    public class GetRoleByIdCommandHandler : IRequestHandler<GetRoleByIdCommand, IResponseWrapper>
    {
        private readonly IRoleService _roleService;
        public GetRoleByIdCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }
        public async Task<IResponseWrapper> Handle(GetRoleByIdCommand request, CancellationToken cancellationToken)
        {
            return await _roleService.GetRoleById(request.RoleId);
        }
    }
}
