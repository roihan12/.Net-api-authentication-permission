using Application.Services.Employees;
using Common.Responses.Wrappers;
using MediatR;


namespace Application.Features.Employees.Commands
{
    public class DeleteEmployeeCommand : IRequest<IResponseWrapper>
    {
       public int EmployeeId { get; set; }
    }

    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;
        public DeleteEmployeeCommandHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        public async Task<IResponseWrapper> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.EmployeeId);
            if (employeeInDb is not null)
            {
                var deletedEmployeeId = await _employeeService.DeleteEmployeeAsync(employeeInDb);
                return await ResponseWrapper<int>.SuccessAsync(deletedEmployeeId, "Employee deleted successfully.");
            }
            return await ResponseWrapper.FailAsync("Employee not found.");
        }
    }

}
