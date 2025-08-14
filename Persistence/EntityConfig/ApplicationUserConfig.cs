using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfig
{
    public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Basic Properties
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.UserType)
                .IsRequired()
                .HasMaxLength(20);

            // Dietitian Properties
            builder.Property(u => u.LicenseNumber)
                .HasMaxLength(255);

            builder.Property(u => u.Specialization)
                .HasMaxLength(100);

            builder.Property(u => u.Bio)
                .HasMaxLength(500);

            // Client Properties
            builder.Property(u => u.Gender)
                .HasMaxLength(10);

            builder.Property(u => u.Height)
                .HasColumnType("decimal(5,2)");

            builder.Property(u => u.InitialWeight)
                .HasColumnType("decimal(5,2)");

            builder.Property(u => u.CurrentWeight)
                .HasColumnType("decimal(5,2)");

            builder.Property(u => u.TargetWeight)
                .HasColumnType("decimal(5,2)");

            builder.Property(u => u.ActivityLevel)
                .HasMaxLength(50);

            builder.Property(u => u.MedicalConditions)
                .HasMaxLength(500);

            builder.Property(u => u.Allergies)
                .HasMaxLength(500);

            builder.Property(u => u.FoodPreferences)
                .HasMaxLength(500);

            builder.Property(u => u.Notes)
                .HasMaxLength(1000);

            // Relationships
            // Client -> Dietitian relationship (self-referencing)
            builder.HasOne(u => u.Dietitian)
                .WithMany(u => u.Clients)
                .HasForeignKey(u => u.DietitianId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Ignore calculated properties
            builder.Ignore(u => u.FullName);
            builder.Ignore(u => u.IsAdmin);
            builder.Ignore(u => u.IsDietitian);
            builder.Ignore(u => u.IsClient);
            builder.Ignore(u => u.Age);
            builder.Ignore(u => u.BMI);
        }
    }
}
