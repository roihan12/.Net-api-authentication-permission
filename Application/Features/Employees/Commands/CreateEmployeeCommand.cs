using Application.Pipelines;
using Application.Services.Employees;
using AutoMapper;
using Common.Request.Employees;
using Common.Responses.Employees;
using Common.Responses.Wrappers;
using Domain;
using MediatR;


namespace Application.Features.Employees.Commands
{
    public class CreateEmployeeCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public CreateEmployeeRequest CreateEmployeeRequest { get; set; }
    }

    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, IResponseWrapper>
    {

        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        public CreateEmployeeCommandHandler(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }
        public async Task<IResponseWrapper> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var mappedEmployee = _mapper.Map<Employee>(request.CreateEmployeeRequest);
            var createdEmployee = await _employeeService.CreateEmployeeAsync(mappedEmployee);
            if (createdEmployee.Id > 0)
            {
                var employeeMap = _mapper.Map<EmployeeResponse>(createdEmployee);
                return await ResponseWrapper<EmployeeResponse>.SuccessAsync(employeeMap, "Employee created successfully.");
            }
            return await ResponseWrapper.FailAsync("Failed to create employee.");
        }
    }
}
