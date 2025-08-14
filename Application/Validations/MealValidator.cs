using Domain.Entities;
using FluentValidation;

namespace Application.Validations
{
    public class MealValidator : AbstractValidator<Meal>
    {
        public MealValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Meal title is required")
                .MaximumLength(200).WithMessage("Meal title cannot be longer than 200 characters")
                .MinimumLength(2).WithMessage("Meal title must be at least 2 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot be longer than 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.MealType)
                .NotEmpty().WithMessage("Meal type is required")
                .MaximumLength(50).WithMessage("Meal type cannot be longer than 50 characters")
                .Must(x => x == "Breakfast" || x == "Lunch" || x == "Dinner" || x == "Snack" || x == "Pre-Workout" || x == "Post-Workout")
                .WithMessage("Invalid meal type");

            RuleFor(x => x.ScheduledTime)
                .NotEmpty().WithMessage("Scheduled time is required")
                .Must(BeValidTime).WithMessage("Invalid time format");

            RuleFor(x => x.Ingredients)
                .MaximumLength(2000).WithMessage("Ingredients cannot be longer than 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.Ingredients));

            RuleFor(x => x.Instructions)
                .MaximumLength(2000).WithMessage("Instructions cannot be longer than 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.Instructions));

            // Nutrition validations
            RuleFor(x => x.Calories)
                .GreaterThanOrEqualTo(0).WithMessage("Calories cannot be less than 0")
                .LessThanOrEqualTo(2000).WithMessage("Calories cannot be greater than 2000")
                .When(x => x.Calories.HasValue);

            RuleFor(x => x.Protein)
                .GreaterThanOrEqualTo(0).WithMessage("Protein cannot be less than 0")
                .LessThanOrEqualTo(200).WithMessage("Protein cannot be greater than 200g")
                .When(x => x.Protein.HasValue);

            RuleFor(x => x.Carbohydrates)
                .GreaterThanOrEqualTo(0).WithMessage("Carbohydrates cannot be less than 0")
                .LessThanOrEqualTo(300).WithMessage("Carbohydrates cannot be greater than 300g")
                .When(x => x.Carbohydrates.HasValue);

            RuleFor(x => x.Fat)
                .GreaterThanOrEqualTo(0).WithMessage("Fat cannot be less than 0")
                .LessThanOrEqualTo(100).WithMessage("Fat cannot be greater than 100g")
                .When(x => x.Fat.HasValue);

            RuleFor(x => x.Fiber)
                .GreaterThanOrEqualTo(0).WithMessage("Fiber cannot be less than 0")
                .LessThanOrEqualTo(50).WithMessage("Fiber cannot be greater than 50g")
                .When(x => x.Fiber.HasValue);

            RuleFor(x => x.Sugar)
                .GreaterThanOrEqualTo(0).WithMessage("Sugar cannot be less than 0")
                .LessThanOrEqualTo(100).WithMessage("Sugar cannot be greater than 100g")
                .When(x => x.Sugar.HasValue);

            RuleFor(x => x.Sodium)
                .GreaterThanOrEqualTo(0).WithMessage("Sodium cannot be less than 0")
                .LessThanOrEqualTo(5000).WithMessage("Sodium cannot be greater than 5000mg")
                .When(x => x.Sodium.HasValue);

            RuleFor(x => x.AllergenInfo)
                .MaximumLength(500).WithMessage("Allergen information cannot be longer than 500 characters")
                .When(x => !string.IsNullOrEmpty(x.AllergenInfo));

            RuleFor(x => x.PreparationTimeMinutes)
                .GreaterThanOrEqualTo(0).WithMessage("Preparation time cannot be less than 0")
                .LessThanOrEqualTo(480).WithMessage("Preparation time cannot exceed 8 hours");

            RuleFor(x => x.ServingSize)
                .GreaterThan(0).WithMessage("Serving size must be greater than 0")
                .LessThanOrEqualTo(20).WithMessage("Serving size cannot exceed 20");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot be longer than 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.DietPlanId)
                .NotEmpty().WithMessage("Diet plan ID is required")
                .Must(BeValidGuid).WithMessage("Invalid diet plan ID format");

            // Business rules
            When(x => x.Calories.HasValue && x.Protein.HasValue && x.Carbohydrates.HasValue && x.Fat.HasValue, () =>
            {
                RuleFor(x => x)
                    .Must(x => ValidateCalorieCalculation(x.Calories!.Value, x.Protein!.Value, x.Carbohydrates!.Value, x.Fat!.Value))
                    .WithMessage("Calorie calculation does not match macronutrients (tolerance: ±50 calories)");
            });

            // Meal type and time validation
            RuleFor(x => x)
                .Must(x => ValidateMealTimeForType(x.MealType, x.ScheduledTime))
                .WithMessage("Meal time is not appropriate for the meal type");
        }

        private static bool BeValidGuid(string guid)
        {
            return Guid.TryParse(guid, out _);
        }

        private static bool BeValidTime(TimeSpan time)
        {
            return time >= TimeSpan.Zero && time < TimeSpan.FromDays(1);
        }

        private static bool ValidateCalorieCalculation(decimal calories, decimal protein, decimal carbs, decimal fat)
        {
            // Protein and carbs: 4 cal/g, Fat: 9 cal/g
            var calculatedCalories = (protein * 4) + (carbs * 4) + (fat * 9);
            var tolerance = 50; // Allow 50 calorie tolerance
            return Math.Abs(calories - calculatedCalories) <= tolerance;
        }

        private static bool ValidateMealTimeForType(string mealType, TimeSpan scheduledTime)
        {
            return mealType switch
            {
                "Breakfast" => scheduledTime >= new TimeSpan(5, 0, 0) && scheduledTime <= new TimeSpan(11, 0, 0),
                "Lunch" => scheduledTime >= new TimeSpan(11, 0, 0) && scheduledTime <= new TimeSpan(15, 0, 0),
                "Dinner" => scheduledTime >= new TimeSpan(17, 0, 0) && scheduledTime <= new TimeSpan(22, 0, 0),
                "Snack" => scheduledTime >= new TimeSpan(6, 0, 0) && scheduledTime <= new TimeSpan(23, 0, 0),
                "Pre-Workout" => scheduledTime >= new TimeSpan(5, 0, 0) && scheduledTime <= new TimeSpan(23, 0, 0),
                "Post-Workout" => scheduledTime >= new TimeSpan(5, 0, 0) && scheduledTime <= new TimeSpan(23, 0, 0),
                _ => true // For unknown types, don't validate time
            };
        }
    }
}