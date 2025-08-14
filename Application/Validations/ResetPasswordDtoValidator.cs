using Application.DTOs;
using FluentValidation;

namespace Application.Validations
{
    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Please enter a valid email address")
                .MaximumLength(255).WithMessage("Email address cannot be longer than 255 characters");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Reset token is required")
                .MaximumLength(500).WithMessage("Token cannot be longer than 500 characters");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters")
                .MaximumLength(100).WithMessage("New password cannot be longer than 100 characters")
                .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("New password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Password confirmation is required")
                .Equal(x => x.NewPassword).WithMessage("New password and confirmation password do not match");
        }
    }
}