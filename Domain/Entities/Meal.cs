namespace Domain.Entities
{
    public class Meal : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string MealType { get; set; } = string.Empty;
        public TimeSpan ScheduledTime { get; set; }
        public string? Ingredients { get; set; }
        public string? Instructions { get; set; }
        public decimal? Calories { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Carbohydrates { get; set; }
        public decimal? Fat { get; set; }
        public decimal? Fiber { get; set; }
        public decimal? Sugar { get; set; }
        public decimal? Sodium { get; set; }
        public string? AllergenInfo { get; set; }
        public int PreparationTimeMinutes { get; set; }
        public int ServingSize { get; set; } = 1;
        public string? Notes { get; set; }

        // Foreign Key
        public string DietPlanId { get; set; } = string.Empty;

        // Navigation Properties
        public virtual DietPlan DietPlan { get; set; } = null!;

        // Calculated Properties
        public string ScheduledTimeFormatted => ScheduledTime.ToString(@"hh\:mm");

        public decimal? CaloriesPerServing =>
            Calories.HasValue && ServingSize > 0 ? Calories / ServingSize : null;
    }
}