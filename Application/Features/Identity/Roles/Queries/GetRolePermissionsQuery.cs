using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Queries
{
    public class GetRolePermissionsQuery : IRequest<IResponseWrapper>
    {
        public string RoleId { get; set; }
    }

    public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, IResponseWrapper>
    {
        private readonly IRoleService _roleService;
        public GetRolePermissionsQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }
        public async Task<IResponseWrapper> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        {
            return await _roleService.GetPermissionsAsync(request.RoleId);
        }
    }
}
