using Application.Utilities.Constants;
using Domain.Entities;
using FluentValidation;

namespace Application.Validations
{
    public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
    {
        public ApplicationUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot be longer than 100 characters")
                .Matches("^[a-zA-Z\\s]+$").WithMessage("First name can only contain letters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot be longer than 100 characters")
                .Matches("^[a-zA-Z\\s]+$").WithMessage("Last name can only contain letters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Please enter a valid email address")
                .MaximumLength(255).WithMessage("Email address cannot be longer than 255 characters");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters")
                .MaximumLength(50).WithMessage("Username cannot be longer than 50 characters")
                .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Username can only contain letters, numbers, dots, underscores and hyphens");

            RuleFor(x => x.UserType)
                .NotEmpty().WithMessage("User type is required")
                .Must(x => x == Roles.Admin || x == Roles.Dietitian || x == Roles.Client)
                .WithMessage("Invalid user type");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            // Dietitian-specific validations
            When(x => x.UserType == Roles.Dietitian, () =>
            {
                RuleFor(x => x.LicenseNumber)
                    .NotEmpty().WithMessage("Dietitian license number is required")
                    .MaximumLength(255).WithMessage("License number cannot be longer than 255 characters");

                RuleFor(x => x.Specialization)
                    .NotEmpty().WithMessage("Specialization is required")
                    .MaximumLength(100).WithMessage("Specialization cannot be longer than 100 characters");

                RuleFor(x => x.YearsOfExperience)
                    .NotNull().WithMessage("Years of experience is required")
                    .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be less than 0")
                    .LessThanOrEqualTo(50).WithMessage("Years of experience cannot be greater than 50");

                RuleFor(x => x.Bio)
                    .MaximumLength(500).WithMessage("Bio cannot be longer than 500 characters")
                    .When(x => !string.IsNullOrEmpty(x.Bio));
            });

            // Client-specific validations
            When(x => x.UserType == Roles.Client, () =>
            {
                RuleFor(x => x.DateOfBirth)
                    .NotNull().WithMessage("Date of birth is required")
                    .LessThan(DateTime.Today).WithMessage("Date of birth must be before today")
                    .GreaterThan(DateTime.Today.AddYears(-120)).WithMessage("Invalid date of birth");

                RuleFor(x => x.Gender)
                    .NotEmpty().WithMessage("Gender is required")
                    .Must(x => x == "Male" || x == "Female" || x == "Other")
                    .WithMessage("Invalid gender value");

                RuleFor(x => x.Height)
                    .NotNull().WithMessage("Height is required")
                    .GreaterThan(50).WithMessage("Height must be greater than 50 cm")
                    .LessThan(300).WithMessage("Height must be less than 300 cm");

                RuleFor(x => x.InitialWeight)
                    .NotNull().WithMessage("Initial weight is required")
                    .GreaterThan(20).WithMessage("Initial weight must be greater than 20 kg")
                    .LessThan(500).WithMessage("Initial weight must be less than 500 kg");

                RuleFor(x => x.CurrentWeight)
                    .NotNull().WithMessage("Current weight is required")
                    .GreaterThan(20).WithMessage("Current weight must be greater than 20 kg")
                    .LessThan(500).WithMessage("Current weight must be less than 500 kg");

                RuleFor(x => x.TargetWeight)
                    .GreaterThan(20).WithMessage("Target weight must be greater than 20 kg")
                    .LessThan(500).WithMessage("Target weight must be less than 500 kg")
                    .When(x => x.TargetWeight.HasValue);

                RuleFor(x => x.ActivityLevel)
                    .Must(x => x == "Sedentary" || x == "Light" || x == "Moderate" || x == "Active" || x == "VeryActive")
                    .WithMessage("Invalid activity level")
                    .When(x => !string.IsNullOrEmpty(x.ActivityLevel));

                RuleFor(x => x.MedicalConditions)
                    .MaximumLength(500).WithMessage("Medical conditions cannot be longer than 500 characters")
                    .When(x => !string.IsNullOrEmpty(x.MedicalConditions));

                RuleFor(x => x.Allergies)
                    .MaximumLength(500).WithMessage("Allergies cannot be longer than 500 characters")
                    .When(x => !string.IsNullOrEmpty(x.Allergies));

                RuleFor(x => x.FoodPreferences)
                    .MaximumLength(500).WithMessage("Food preferences cannot be longer than 500 characters")
                    .When(x => !string.IsNullOrEmpty(x.FoodPreferences));

                RuleFor(x => x.Notes)
                    .MaximumLength(1000).WithMessage("Notes cannot be longer than 1000 characters")
                    .When(x => !string.IsNullOrEmpty(x.Notes));
            });

            // DietitianId validation for clients
            When(x => x.UserType == Roles.Client && !string.IsNullOrEmpty(x.DietitianId), () =>
            {
                RuleFor(x => x.DietitianId)
                    .Must(BeValidGuid).WithMessage("Invalid dietitian ID format");
            });
        }

        private static bool BeValidGuid(string? guid)
        {
            return !string.IsNullOrEmpty(guid) && Guid.TryParse(guid, out _);
        }
    }
}