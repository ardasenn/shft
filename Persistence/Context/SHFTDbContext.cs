using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityConfig;

namespace Persistence.Context
{
    public class SHFTDbContext : IdentityDbContext<ApplicationUser>
    {
        public SHFTDbContext(DbContextOptions<SHFTDbContext> options)
            : base(options) { }

        // DbSets for our entities
        public DbSet<DietPlan> DietPlans { get; set; }
        public DbSet<Meal> Meals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new ApplicationUserConfig());
            modelBuilder.ApplyConfiguration(new DietPlanConfig());
            modelBuilder.ApplyConfiguration(new MealConfig());

            Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {
            DataGenerator.Initialize(modelBuilder);
        }
    }
}
