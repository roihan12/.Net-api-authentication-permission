using Application.Services.Identity;
using Common.Request.Identity;
using Common.Responses.Identity;
using FluentValidation;

namespace Application.Features.Identity.Users.Validators
{
    public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequest>
    {
        public UserRegistrationRequestValidator(IUserService userService)
        {
            RuleFor(request => request.Email)
                .MustAsync(async (email, cancellation) =>
                    await userService.GetUserByEmailAsync(email) is not UserResponse existingUser
                ).WithMessage("A user with the specified email already exists.");
            RuleFor(request => request.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");
            RuleFor(request => request.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");
            RuleFor(request => request.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(100)
                .EmailAddress().WithMessage("A valid email is required.");
            RuleFor(request => request.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");
            RuleFor(request => request.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
            RuleFor(request => request.ConfirmPassword)
                .Equal(request => request.Password).WithMessage("Passwords do not match.");
            RuleFor(request => request.PhoneNumber)
                .MaximumLength(15).WithMessage("Phone number must not exceed 15 characters.");
        }
    }
}
