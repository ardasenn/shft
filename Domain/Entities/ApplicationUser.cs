using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Clients = new HashSet<ApplicationUser>();
            CreatedDietPlans = new HashSet<DietPlan>();
            DietPlans = new HashSet<DietPlan>();
        }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty; // Admin, Dietitian, Client
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Status Status { get; set; } = Status.Active;

        // Dietitian-specific properties (only filled if UserType = "Dietitian")
        public string? LicenseNumber { get; set; }
        public string? Specialization { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Bio { get; set; }

        // Client-specific properties (only filled if UserType = "Client")
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; } // Male, Female, Other
        public decimal? Height { get; set; } // in cm
        public decimal? InitialWeight { get; set; } // in kg
        public decimal? CurrentWeight { get; set; } // in kg
        public decimal? TargetWeight { get; set; } // in kg
        public string? ActivityLevel { get; set; } // Sedentary, Light, Moderate, Active, Very Active
        public string? MedicalConditions { get; set; }
        public string? Allergies { get; set; }
        public string? FoodPreferences { get; set; }
        public string? Notes { get; set; }

        // Foreign Key for Client -> Dietitian relationship
        public string? DietitianId { get; set; }

        // Navigation Properties
        // For Dietitians: clients assigned to them
        public virtual ICollection<ApplicationUser> Clients { get; set; } = new List<ApplicationUser>();
        // For Dietitians: diet plans they created
        public virtual ICollection<DietPlan> CreatedDietPlans { get; set; } = new List<DietPlan>();
        // For Clients: their diet plans
        public virtual ICollection<DietPlan> DietPlans { get; set; } = new List<DietPlan>();
        // For Clients: their dietitian
        public virtual ApplicationUser? Dietitian { get; set; }

        // Calculated Properties
        public string FullName => $"{FirstName} {LastName}";
        public bool IsAdmin => UserType == "Admin";
        public bool IsDietitian => UserType == "Dietitian";
        public bool IsClient => UserType == "Client";

        // Client-specific calculated properties
        public int? Age => DateOfBirth.HasValue
            ? DateTime.Today.Year
                - DateOfBirth.Value.Year
                - (DateTime.Today.DayOfYear < DateOfBirth.Value.DayOfYear ? 1 : 0)
            : null;

        public decimal? BMI => Height > 0 && CurrentWeight > 0
            ? CurrentWeight / ((Height / 100) * (Height / 100))
            : null;
    }
}