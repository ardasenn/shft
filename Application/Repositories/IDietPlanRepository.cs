using Domain.Entities;

namespace Application.Repositories
{
    public interface IDietPlanRepository : IReadRepository<DietPlan>, IWriteRepository<DietPlan>
    {
        Task<IList<DietPlan>> GetPlansByClientAsync(string clientId);
        Task<IList<DietPlan>> GetPlansByDietitianAsync(string dietitianId);
        Task<IList<DietPlan>> GetActivePlansAsync();
        Task<IList<DietPlan>> GetPlansEndingInDaysAsync(int days);
        Task<DietPlan?> GetPlanWithMealsAsync(string planId);
        Task<bool> ActivatePlanAsync(string planId);
        Task<bool> DeactivatePlanAsync(string planId);
        Task<List<DietPlan>> GetPlansAsync(bool includeRelated = false);
    }
}
