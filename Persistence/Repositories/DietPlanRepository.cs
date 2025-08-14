using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class DietPlanRepository : ReadRepository<DietPlan>, IDietPlanRepository
    {
        private readonly SHFTDbContext _context;

        public DietPlanRepository(SHFTDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IList<DietPlan>> GetPlansByClientAsync(string clientId)
        {
            return await _context.DietPlans
                .Where(dp => dp.ClientId == clientId && dp.Status != Domain.Enums.Status.Pasive)
                .Include(dp => dp.Client)
                .Include(dp => dp.Dietitian)
                .OrderByDescending(dp => dp.CreationDate)
                .ToListAsync();
        }

        public async Task<IList<DietPlan>> GetPlansByDietitianAsync(string dietitianId)
        {
            return await _context.DietPlans
                .Where(dp => dp.DietitianId == dietitianId && dp.Status != Domain.Enums.Status.Pasive)
                .Include(dp => dp.Client)
                .Include(dp => dp.Dietitian)
                .OrderByDescending(dp => dp.CreationDate)
                .ToListAsync();
        }

        public async Task<IList<DietPlan>> GetActivePlansAsync()
        {
            var today = DateTime.Today;
            return await _context.DietPlans
                .Where(dp => dp.IsActive &&
                           dp.StartDate <= today &&
                           dp.EndDate >= today &&
                           dp.Status != Domain.Enums.Status.Pasive)
                .Include(dp => dp.Client)
                .Include(dp => dp.Dietitian)
                .OrderByDescending(dp => dp.CreationDate)
                .ToListAsync();
        }

        public async Task<IList<DietPlan>> GetPlansEndingInDaysAsync(int days)
        {
            var targetDate = DateTime.Today.AddDays(days);
            return await _context.DietPlans
                .Where(dp => dp.IsActive &&
                           dp.EndDate <= targetDate &&
                           dp.EndDate >= DateTime.Today &&
                           dp.Status != Domain.Enums.Status.Pasive)
                .Include(dp => dp.Client)
                .Include(dp => dp.Dietitian)
                .OrderBy(dp => dp.EndDate)
                .ToListAsync();
        }

        public async Task<DietPlan?> GetPlanWithMealsAsync(string planId)
        {
            return await _context.DietPlans
                .Where(dp => dp.Id == planId && dp.Status != Domain.Enums.Status.Pasive)
                .Include(dp => dp.Client)
                .Include(dp => dp.Dietitian)
                .Include(dp => dp.Meals.Where(m => m.Status != Domain.Enums.Status.Pasive))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ActivatePlanAsync(string planId)
        {
            var plan = await _context.DietPlans.FindAsync(planId);
            if (plan != null)
            {
                plan.IsActive = true;
                plan.UpdateDate = DateTime.UtcNow;
                plan.Status = Domain.Enums.Status.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeactivatePlanAsync(string planId)
        {
            var plan = await _context.DietPlans.FindAsync(planId);
            if (plan != null)
            {
                plan.IsActive = false;
                plan.UpdateDate = DateTime.UtcNow;
                plan.Status = Domain.Enums.Status.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<DietPlan>> GetPlansAsync(bool includeRelated = false)
        {
            var query = _context.DietPlans
                .Where(dp => dp.Status != Domain.Enums.Status.Pasive);

            if (includeRelated)
            {
                query = query
                    .Include(dp => dp.Client)
                    .Include(dp => dp.Dietitian);
            }

            return await query
                .OrderByDescending(dp => dp.CreationDate)
                .ToListAsync();
        }

        // Implement WriteRepository methods
        public async Task<bool> AddAsync(DietPlan entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.CreationDate = DateTime.UtcNow;
            entity.Status = Domain.Enums.Status.Active;
            await _context.DietPlans.AddAsync(entity);
            return true;
        }

        public async Task<bool> AddRangeAsync(List<DietPlan> entities)
        {
            foreach (var entity in entities)
            {
                entity.Id = Guid.NewGuid().ToString();
                entity.CreationDate = DateTime.UtcNow;
                entity.Status = Domain.Enums.Status.Active;
            }
            await _context.DietPlans.AddRangeAsync(entities);
            return true;
        }

        public bool Remove(DietPlan entity)
        {
            entity.Status = Domain.Enums.Status.Pasive;
            entity.DeleteDate = DateTime.UtcNow;
            _context.DietPlans.Update(entity);
            return true;
        }

        public bool Delete(DietPlan entity)
        {
            _context.DietPlans.Remove(entity);
            return true;
        }

        public bool RemoveRange(List<DietPlan> entities)
        {
            foreach (var entity in entities)
            {
                entity.Status = Domain.Enums.Status.Pasive;
                entity.DeleteDate = DateTime.UtcNow;
            }
            _context.DietPlans.UpdateRange(entities);
            return true;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var entity = await _context.DietPlans.FindAsync(id);
            if (entity != null)
            {
                return Remove(entity);
            }
            return false;
        }

        public bool Update(DietPlan entity)
        {
            entity.UpdateDate = DateTime.UtcNow;
            entity.Status = Domain.Enums.Status.Modified;
            _context.DietPlans.Update(entity);
            return true;
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
