using Domain.Entities;
using FluentValidation;

namespace Application.Validations
{
    public class DietPlanValidator : AbstractValidator<DietPlan>
    {
        public DietPlanValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Plan title is required")
                .MaximumLength(200).WithMessage("Plan title cannot be longer than 200 characters")
                .MinimumLength(3).WithMessage("Plan title must be at least 3 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot be longer than 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");

            RuleFor(x => x)
                .Must(x => (x.EndDate - x.StartDate).Days >= 1)
                .WithMessage("Plan duration must be at least 1 day")
                .Must(x => (x.EndDate - x.StartDate).Days <= 365)
                .WithMessage("Plan duration cannot exceed 365 days");

            RuleFor(x => x.InitialWeight)
                .GreaterThan(20).WithMessage("Initial weight must be greater than 20 kg")
                .LessThan(500).WithMessage("Initial weight must be less than 500 kg")
                .When(x => x.InitialWeight.HasValue);

            RuleFor(x => x.TargetWeight)
                .GreaterThan(20).WithMessage("Target weight must be greater than 20 kg")
                .LessThan(500).WithMessage("Target weight must be less than 500 kg")
                .When(x => x.TargetWeight.HasValue);

            RuleFor(x => x.DailyCalorieTarget)
                .GreaterThan(800).WithMessage("Daily calorie target must be greater than 800")
                .LessThan(5000).WithMessage("Daily calorie target must be less than 5000")
                .When(x => x.DailyCalorieTarget.HasValue);

            RuleFor(x => x.PlanType)
                .NotEmpty().WithMessage("Plan type is required")
                .MaximumLength(50).WithMessage("Plan type cannot be longer than 50 characters")
                .Must(x => x == "WeightLoss" || x == "WeightGain" || x == "Maintenance" || x == "Muscle Building" || x == "General Health")
                .WithMessage("Invalid plan type");

            RuleFor(x => x.SpecialInstructions)
                .MaximumLength(1000).WithMessage("Special instructions cannot be longer than 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.SpecialInstructions));

            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage("Client ID is required")
                .Must(BeValidGuid).WithMessage("Invalid client ID format");

            RuleFor(x => x.DietitianId)
                .NotEmpty().WithMessage("Dietitian ID is required")
                .Must(BeValidGuid).WithMessage("Invalid dietitian ID format");

            // Business rule: Client and Dietitian cannot be the same
            RuleFor(x => x)
                .Must(x => x.ClientId != x.DietitianId)
                .WithMessage("Client and dietitian cannot be the same person");

            // Logical weight validation
            When(x => x.InitialWeight.HasValue && x.TargetWeight.HasValue, () =>
            {
                RuleFor(x => x)
                    .Must(x => Math.Abs(x.InitialWeight!.Value - x.TargetWeight!.Value) >= 1)
                    .WithMessage("There must be at least 1 kg difference between initial and target weight")
                    .Must(x => Math.Abs(x.InitialWeight!.Value - x.TargetWeight!.Value) <= 100)
                    .WithMessage("There cannot be more than 100 kg difference between initial and target weight");
            });
        }

        private static bool BeValidGuid(string guid)
        {
            return Guid.TryParse(guid, out _);
        }
    }
}