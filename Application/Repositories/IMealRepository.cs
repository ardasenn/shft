using Domain.Entities;

namespace Application.Repositories
{
    public interface IMealRepository : IReadRepository<Meal>, IWriteRepository<Meal>
    {
        Task<IList<Meal>> GetMealsByDietPlanAsync(string dietPlanId);
        Task<IList<Meal>> GetMealsByTypeAsync(string mealType);
        Task<IList<Meal>> GetMealsForDateAsync(string dietPlanId, DateTime date);
        Task<IList<Meal>> GetMealsByTimeRangeAsync(
            string dietPlanId,
            TimeSpan startTime,
            TimeSpan endTime);
        Task<decimal?> GetTotalCaloriesForPlanAsync(string dietPlanId);
        Task<decimal?> GetTotalCaloriesForDateAsync(string dietPlanId, DateTime date);
    }
}