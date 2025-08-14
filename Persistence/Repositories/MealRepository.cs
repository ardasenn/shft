using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class MealRepository : ReadRepository<Meal>, IMealRepository
    {
        private readonly SHFTDbContext _context;

        public MealRepository(SHFTDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IList<Meal>> GetMealsByDietPlanAsync(string dietPlanId)
        {
            return await _context.Meals
                .Where(m => m.DietPlanId == dietPlanId && m.Status != Domain.Enums.Status.Pasive)
                .Include(m => m.DietPlan)
                .OrderBy(m => m.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IList<Meal>> GetMealsByTypeAsync(string mealType)
        {
            return await _context.Meals
                .Where(m => m.MealType == mealType && m.Status != Domain.Enums.Status.Pasive)
                .Include(m => m.DietPlan)
                .OrderBy(m => m.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IList<Meal>> GetMealsForDateAsync(string dietPlanId, DateTime date)
        {
            // Since meals don't have a specific date, we return all meals for the plan
            // In a more advanced system, you might have daily meal schedules
            return await GetMealsByDietPlanAsync(dietPlanId);
        }

        public async Task<IList<Meal>> GetMealsByTimeRangeAsync(string dietPlanId, TimeSpan startTime, TimeSpan endTime)
        {
            return await _context.Meals
                .Where(m => m.DietPlanId == dietPlanId &&
                           m.ScheduledTime >= startTime &&
                           m.ScheduledTime <= endTime &&
                           m.Status != Domain.Enums.Status.Pasive)
                .Include(m => m.DietPlan)
                .OrderBy(m => m.ScheduledTime)
                .ToListAsync();
        }

        public async Task<decimal?> GetTotalCaloriesForPlanAsync(string dietPlanId)
        {
            return await _context.Meals
                .Where(m => m.DietPlanId == dietPlanId &&
                           m.Status != Domain.Enums.Status.Pasive &&
                           m.Calories.HasValue)
                .SumAsync(m => m.Calories);
        }

        public async Task<decimal?> GetTotalCaloriesForDateAsync(string dietPlanId, DateTime date)
        {
            // For now, return total calories for the plan
            // In a more advanced system, you would filter by date
            return await GetTotalCaloriesForPlanAsync(dietPlanId);
        }



        // Implement WriteRepository methods
        public async Task<bool> AddAsync(Meal entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.CreationDate = DateTime.UtcNow;
            entity.Status = Domain.Enums.Status.Active;
            await _context.Meals.AddAsync(entity);
            return true;
        }

        public async Task<bool> AddRangeAsync(List<Meal> entities)
        {
            foreach (var entity in entities)
            {
                entity.Id = Guid.NewGuid().ToString();
                entity.CreationDate = DateTime.UtcNow;
                entity.Status = Domain.Enums.Status.Active;
            }
            await _context.Meals.AddRangeAsync(entities);
            return true;
        }

        public bool Remove(Meal entity)
        {
            entity.Status = Domain.Enums.Status.Pasive;
            entity.DeleteDate = DateTime.UtcNow;
            _context.Meals.Update(entity);
            return true;
        }

        public bool Delete(Meal entity)
        {
            _context.Meals.Remove(entity);
            return true;
        }

        public bool RemoveRange(List<Meal> entities)
        {
            foreach (var entity in entities)
            {
                entity.Status = Domain.Enums.Status.Pasive;
                entity.DeleteDate = DateTime.UtcNow;
            }
            _context.Meals.UpdateRange(entities);
            return true;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var entity = await _context.Meals.FindAsync(id);
            if (entity != null)
            {
                return Remove(entity);
            }
            return false;
        }

        public bool Update(Meal entity)
        {
            entity.UpdateDate = DateTime.UtcNow;
            entity.Status = Domain.Enums.Status.Modified;
            _context.Meals.Update(entity);
            return true;
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
