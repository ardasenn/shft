using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfig
{
    public class DietPlanConfig : BaseConfig<DietPlan>
    {
        public override void Configure(EntityTypeBuilder<DietPlan> builder)
        {
            base.Configure(builder);

            // Properties
            builder.Property(dp => dp.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(dp => dp.Description)
                .HasMaxLength(1000);

            builder.Property(dp => dp.StartDate)
                .IsRequired();

            builder.Property(dp => dp.EndDate)
                .IsRequired();

            builder.Property(dp => dp.InitialWeight)
                .HasColumnType("decimal(5,2)");

            builder.Property(dp => dp.TargetWeight)
                .HasColumnType("decimal(5,2)");

            builder.Property(dp => dp.DailyCalorieTarget)
                .HasColumnType("decimal(7,2)");

            builder.Property(dp => dp.PlanType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(dp => dp.SpecialInstructions)
                .HasMaxLength(1000);

            builder.Property(dp => dp.ClientId)
                .IsRequired();

            builder.Property(dp => dp.DietitianId)
                .IsRequired();

            // Relationships
            // DietPlan -> Client relationship
            builder.HasOne(dp => dp.Client)
                .WithMany(u => u.DietPlans)
                .HasForeignKey(dp => dp.ClientId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // DietPlan -> Dietitian relationship  
            builder.HasOne(dp => dp.Dietitian)
                .WithMany(u => u.CreatedDietPlans)
                .HasForeignKey(dp => dp.DietitianId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // DietPlan -> Meals relationship
            builder.HasMany(dp => dp.Meals)
                .WithOne(m => m.DietPlan)
                .HasForeignKey(m => m.DietPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore calculated properties
            builder.Ignore(dp => dp.DurationInDays);
            builder.Ignore(dp => dp.IsCurrentlyActive);
        }
    }
}
