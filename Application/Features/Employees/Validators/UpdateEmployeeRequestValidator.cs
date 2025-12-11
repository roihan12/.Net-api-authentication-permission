using Common.Request.Employees;
using FluentValidation;

namespace Application.Features.Employees.Validators
{
    internal class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
    {
        public UpdateEmployeeRequestValidator()
        {
            RuleFor(request => request.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(request => request.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(request => request.Email).NotEmpty()
                .WithMessage("Email is required.").MaximumLength(100)
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(request => request.Salary)
                .GreaterThanOrEqualTo(0).WithMessage("Salary must be a non-negative value.")
                .NotEmpty().WithMessage("Salary is required.");
        }
    }
}
