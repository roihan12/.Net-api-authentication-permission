

using Application.Services.Identity;
using Common.Request;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Idenity.Queries
{
    public class GetTokenQuery :IRequest<IResponseWrapper>
    {
        public TokenRequest TokenRequest { get; set; }
    }

    public class GetTokenQueryHandler : IRequestHandler<GetTokenQuery, IResponseWrapper>
    {
        // Handler implementation would go here
        private readonly ITokenService _tokenService;

        public GetTokenQueryHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<IResponseWrapper> Handle(GetTokenQuery request, CancellationToken cancellationToken)
        {
           return await _tokenService.GetTokenAsync(request.TokenRequest);
        }

    }
}
