using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfig
{
    public class MealConfig : BaseConfig<Meal>
    {
        public override void Configure(EntityTypeBuilder<Meal> builder)
        {
            base.Configure(builder);

            // Properties
            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            builder.Property(m => m.MealType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.ScheduledTime)
                .IsRequired();

            builder.Property(m => m.Ingredients)
                .HasMaxLength(2000);

            builder.Property(m => m.Instructions)
                .HasMaxLength(2000);

            builder.Property(m => m.Calories)
                .HasColumnType("decimal(7,2)");

            builder.Property(m => m.Protein)
                .HasColumnType("decimal(5,2)");

            builder.Property(m => m.Carbohydrates)
                .HasColumnType("decimal(5,2)");

            builder.Property(m => m.Fat)
                .HasColumnType("decimal(5,2)");

            builder.Property(m => m.Fiber)
                .HasColumnType("decimal(5,2)");

            builder.Property(m => m.Sugar)
                .HasColumnType("decimal(5,2)");

            builder.Property(m => m.Sodium)
                .HasColumnType("decimal(5,2)");

            builder.Property(m => m.AllergenInfo)
                .HasMaxLength(500);

            builder.Property(m => m.Notes)
                .HasMaxLength(1000);

            builder.Property(m => m.DietPlanId)
                .IsRequired();

            // Relationships
            // Meal -> DietPlan relationship
            builder.HasOne(m => m.DietPlan)
                .WithMany(dp => dp.Meals)
                .HasForeignKey(m => m.DietPlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Ignore calculated properties
            builder.Ignore(m => m.ScheduledTimeFormatted);
            builder.Ignore(m => m.CaloriesPerServing);
        }
    }
}
