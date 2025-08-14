using Application.Utilities.Response;
using Domain.Entities;

namespace Application.Services
{
    public interface IDietPlanService
    {
        Task<GenericResponse<List<DietPlan>>> GetAllDietPlansAsync();
        Task<GenericResponse<DietPlan>> GetDietPlanByIdAsync(string id);
        Task<GenericResponse<List<DietPlan>>> GetDietPlansByClientAsync(string clientId);
        Task<GenericResponse<List<DietPlan>>> GetDietPlansByDietitianAsync(string dietitianId);
        Task<GenericResponse<List<DietPlan>>> GetActiveDietPlansAsync();
        Task<GenericResponse<List<DietPlan>>> GetDietPlansEndingInDaysAsync(int days);
        Task<GenericResponse<DietPlan>> GetDietPlanWithMealsAsync(string planId);
        Task<GenericResponse<DietPlan>> CreateDietPlanAsync(DietPlan dietPlan);
        Task<GenericResponse<DietPlan>> UpdateDietPlanAsync(DietPlan dietPlan);
        Task<GenericResponse<string>> DeleteDietPlanAsync(string id);
        Task<GenericResponse<string>> ActivateDietPlanAsync(string planId);
        Task<GenericResponse<string>> DeactivateDietPlanAsync(string planId);
        Task<GenericResponse<DietPlan>> CloneDietPlanAsync(string planId, string newClientId);
        Task<GenericResponse<Dictionary<string, decimal>>> GetDietPlanStatisticsAsync(string planId);
    }
}
