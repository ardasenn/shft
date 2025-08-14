using Application.Utilities.Response;
using Domain.Entities;

namespace Application.Services
{
    public interface IMealService
    {
        Task<GenericResponse<List<Meal>>> GetAllMealsAsync();
        Task<GenericResponse<Meal>> GetMealByIdAsync(string id);
        Task<GenericResponse<List<Meal>>> GetMealsByDietPlanAsync(string dietPlanId);
        Task<GenericResponse<List<Meal>>> GetMealsByTypeAsync(string mealType);
        Task<GenericResponse<List<Meal>>> GetMealsForDateAsync(string dietPlanId, DateTime date);
        Task<GenericResponse<List<Meal>>> GetMealsByTimeRangeAsync(string dietPlanId, TimeSpan startTime, TimeSpan endTime);
        Task<GenericResponse<Meal>> CreateMealAsync(Meal meal);
        Task<GenericResponse<Meal>> UpdateMealAsync(Meal meal);
        Task<GenericResponse<string>> DeleteMealAsync(string id);
        Task<GenericResponse<List<Meal>>> CreateMealPlanAsync(string dietPlanId, List<Meal> meals);
        Task<GenericResponse<Dictionary<string, decimal>>> GetMealNutritionSummaryAsync(string mealId);
        Task<GenericResponse<Dictionary<string, decimal>>> GetDailyNutritionSummaryAsync(string dietPlanId, DateTime date);
        Task<GenericResponse<decimal>> GetTotalCaloriesForPlanAsync(string dietPlanId);
        Task<GenericResponse<decimal>> GetTotalCaloriesForDateAsync(string dietPlanId, DateTime date);
    }
}
