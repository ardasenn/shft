using Application.Utilities.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataGenerator
    {
        public static void Initialize(ModelBuilder modelBuilder)
        {
            var adminRoleId = Guid.NewGuid().ToString();
            var dietitianRoleId = Guid.NewGuid().ToString();
            var clientRoleId = Guid.NewGuid().ToString();
            var adminUserId = Guid.NewGuid().ToString();
            var dietitianUserId = Guid.NewGuid().ToString();
            var clientUserId = Guid.NewGuid().ToString();

            // Create Roles
            modelBuilder
                .Entity<IdentityRole>()
                .HasData(
                    new IdentityRole
                    {
                        Id = adminRoleId,
                        Name = Roles.Admin,
                        NormalizedName = Roles.Admin.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                    },
                    new IdentityRole
                    {
                        Id = dietitianRoleId,
                        Name = Roles.Dietitian,
                        NormalizedName = Roles.Dietitian.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                    },
                    new IdentityRole
                    {
                        Id = clientRoleId,
                        Name = Roles.Client,
                        NormalizedName = Roles.Client.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                    }
                );

            // Create Users
            // Password: Admin@123
            var adminPasswordHash = "AQAAAAEAACcQAAAAEGKKvPMb8Y8tJQXUv7fJjKGHLJGKJHGKJHGKJHGKJHGKJHGKJHGKJHGKJHGKJHGKJHGK==";

            modelBuilder
                .Entity<ApplicationUser>()
                .HasData(
                    new ApplicationUser
                    {
                        Id = adminUserId,
                        UserName = "admin",
                        NormalizedUserName = "ADMIN",
                        Email = "admin@dietmanagement.com",
                        NormalizedEmail = "ADMIN@DIETMANAGEMENT.COM",
                        EmailConfirmed = true,
                        PasswordHash = adminPasswordHash,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        FirstName = "System",
                        LastName = "Administrator",
                        UserType = "Admin",
                        CreationDate = DateTime.UtcNow,
                        Status = Status.Active
                    },
                    new ApplicationUser
                    {
                        Id = dietitianUserId,
                        UserName = "dietitian",
                        NormalizedUserName = "DIETITIAN",
                        Email = "dietitian@dietmanagement.com",
                        NormalizedEmail = "DIETITIAN@DIETMANAGEMENT.COM",
                        EmailConfirmed = true,
                        PasswordHash = adminPasswordHash,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        FirstName = "Dr. Jane",
                        LastName = "Smith",
                        UserType = "Dietitian",
                        LicenseNumber = "DT-2024-001",
                        Specialization = "Clinical Nutrition",
                        YearsOfExperience = 5,
                        Bio = "Experienced clinical dietitian specializing in weight management and metabolic disorders.",
                        CreationDate = DateTime.UtcNow,
                        Status = Status.Active
                    },
                    new ApplicationUser
                    {
                        Id = clientUserId,
                        UserName = "johndoe",
                        NormalizedUserName = "JOHNDOE",
                        Email = "john.doe@email.com",
                        NormalizedEmail = "JOHN.DOE@EMAIL.COM",
                        EmailConfirmed = true,
                        PasswordHash = adminPasswordHash,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        FirstName = "John",
                        LastName = "Doe",
                        UserType = "Client",
                        DateOfBirth = new DateTime(1990, 5, 15),
                        Gender = "Male",
                        Height = 175.0m,
                        InitialWeight = 85.0m,
                        CurrentWeight = 85.0m,
                        TargetWeight = 75.0m,
                        ActivityLevel = "Moderate",
                        MedicalConditions = "None",
                        Allergies = "Nuts",
                        FoodPreferences = "No specific preferences",
                        DietitianId = dietitianUserId,
                        CreationDate = DateTime.UtcNow,
                        Status = Status.Active
                    }
                );

            // Add user-role relationships
            modelBuilder
                .Entity<IdentityUserRole<string>>()
                .HasData(
                    new IdentityUserRole<string> { RoleId = adminRoleId, UserId = adminUserId },
                    new IdentityUserRole<string> { RoleId = dietitianRoleId, UserId = dietitianUserId },
                    new IdentityUserRole<string> { RoleId = clientRoleId, UserId = clientUserId }
                );
        }
    }
}
