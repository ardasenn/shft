namespace Application.DTOs
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty; // Admin, Dietitian, Client
        public string? PhoneNumber { get; set; }

        // Dietitian-specific properties
        public string? LicenseNumber { get; set; }
        public string? Specialization { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Bio { get; set; }

        // Client-specific properties
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public decimal? Height { get; set; }
        public decimal? InitialWeight { get; set; }
        public decimal? CurrentWeight { get; set; }
        public decimal? TargetWeight { get; set; }
        public string? ActivityLevel { get; set; }
        public string? MedicalConditions { get; set; }
        public string? Allergies { get; set; }
        public string? FoodPreferences { get; set; }
        public string? Notes { get; set; }
        public string? DietitianId { get; set; }
    }
}
