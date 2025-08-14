using Domain.Entities;

namespace Domain.Entities
{
    public class DietPlan : BaseEntity
    {
        public DietPlan()
        {
            Meals = new HashSet<Meal>();
        }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? InitialWeight { get; set; }
        public decimal? TargetWeight { get; set; }
        public decimal? DailyCalorieTarget { get; set; }
        public string PlanType { get; set; } = string.Empty;
        public string? SpecialInstructions { get; set; }
        public bool IsActive { get; set; } = true;

        // Foreign Keys
        public string ClientId { get; set; } = string.Empty;
        public string DietitianId { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ApplicationUser Client { get; set; } = null!;
        public virtual ApplicationUser Dietitian { get; set; } = null!;
        public virtual ICollection<Meal> Meals { get; set; }

        // Calculated Properties
        public int DurationInDays => (EndDate - StartDate).Days + 1;

        public bool IsCurrentlyActive =>
            IsActive && DateTime.Today >= StartDate && DateTime.Today <= EndDate;
    }
}