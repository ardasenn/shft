using Application.Repositories;
using Application.Services;
using Application.Utilities.Constants;
using Application.Utilities.Response;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Services
{
    public class DietPlanService : IDietPlanService
    {
        private readonly IDietPlanRepository _dietPlanRepository;
        private readonly IApplicationUserRepository _userRepository;

        public DietPlanService(
            IDietPlanRepository dietPlanRepository,
            IApplicationUserRepository userRepository)
        {
            _dietPlanRepository = dietPlanRepository;
            _userRepository = userRepository;
        }

        public async Task<GenericResponse<List<DietPlan>>> GetAllDietPlansAsync()
        {
            try
            {
                var dietPlans = await _dietPlanRepository.GetPlansAsync(includeRelated: true);
                return GenericResponse<List<DietPlan>>.Success(dietPlans, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<DietPlan>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<DietPlan>> GetDietPlanByIdAsync(string id)
        {
            try
            {
                var dietPlan = await _dietPlanRepository.GetByIdAsync(id);
                if (dietPlan == null)
                {
                    return GenericResponse<DietPlan>.Fail(Messages.DietPlanNotFound, 404);
                }

                return GenericResponse<DietPlan>.Success(dietPlan, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<DietPlan>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<DietPlan>>> GetDietPlansByClientAsync(string clientId)
        {
            try
            {
                var dietPlans = await _dietPlanRepository.GetPlansByClientAsync(clientId);
                return GenericResponse<List<DietPlan>>.Success(dietPlans.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<DietPlan>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<DietPlan>>> GetDietPlansByDietitianAsync(string dietitianId)
        {
            try
            {
                var dietPlans = await _dietPlanRepository.GetPlansByDietitianAsync(dietitianId);
                return GenericResponse<List<DietPlan>>.Success(dietPlans.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<DietPlan>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<DietPlan>>> GetActiveDietPlansAsync()
        {
            try
            {
                var activePlans = await _dietPlanRepository.GetActivePlansAsync();
                return GenericResponse<List<DietPlan>>.Success(activePlans.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<DietPlan>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<DietPlan>>> GetDietPlansEndingInDaysAsync(int days)
        {
            try
            {
                var endingPlans = await _dietPlanRepository.GetPlansEndingInDaysAsync(days);
                return GenericResponse<List<DietPlan>>.Success(endingPlans.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<DietPlan>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<DietPlan>> GetDietPlanWithMealsAsync(string planId)
        {
            try
            {
                var dietPlan = await _dietPlanRepository.GetPlanWithMealsAsync(planId);
                if (dietPlan == null)
                {
                    return GenericResponse<DietPlan>.Fail(Messages.DietPlanNotFound, 404);
                }

                return GenericResponse<DietPlan>.Success(dietPlan, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<DietPlan>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<DietPlan>> CreateDietPlanAsync(DietPlan dietPlan)
        {
            try
            {
                // Validate business rules
                var validationResult = await ValidateDietPlanAsync(dietPlan);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                // Check for overlapping plans
                var existingPlans = await _dietPlanRepository.GetPlansByClientAsync(dietPlan.ClientId);
                var hasOverlap = existingPlans.Any(p =>
                    p.Status == Status.Active &&
                    ((dietPlan.StartDate >= p.StartDate && dietPlan.StartDate <= p.EndDate) ||
                     (dietPlan.EndDate >= p.StartDate && dietPlan.EndDate <= p.EndDate) ||
                     (dietPlan.StartDate <= p.StartDate && dietPlan.EndDate >= p.EndDate)));

                if (hasOverlap)
                {
                    return GenericResponse<DietPlan>.Fail(Messages.PlanAlreadyExists, 400);
                }

                await _dietPlanRepository.AddAsync(dietPlan);
                await _dietPlanRepository.SaveAsync();

                return GenericResponse<DietPlan>.Success(dietPlan, Messages.DietPlanCreated);
            }
            catch (Exception ex)
            {
                return GenericResponse<DietPlan>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<DietPlan>> UpdateDietPlanAsync(DietPlan dietPlan)
        {
            try
            {
                var existingPlan = await _dietPlanRepository.GetByIdAsync(dietPlan.Id);
                if (existingPlan == null)
                {
                    return GenericResponse<DietPlan>.Fail(Messages.DietPlanNotFound, 404);
                }

                // Validate business rules
                var validationResult = await ValidateDietPlanAsync(dietPlan);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                _dietPlanRepository.Update(dietPlan);
                await _dietPlanRepository.SaveAsync();

                return GenericResponse<DietPlan>.Success(dietPlan, Messages.DietPlanUpdated);
            }
            catch (Exception ex)
            {
                return GenericResponse<DietPlan>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> DeleteDietPlanAsync(string id)
        {
            try
            {
                var dietPlan = await _dietPlanRepository.GetByIdAsync(id);
                if (dietPlan == null)
                {
                    return GenericResponse<string>.Fail(Messages.DietPlanNotFound, 404);
                }

                await _dietPlanRepository.RemoveAsync(id);
                await _dietPlanRepository.SaveAsync();

                return GenericResponse<string>.Success(id, Messages.DietPlanDeleted);
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> ActivateDietPlanAsync(string planId)
        {
            try
            {
                var success = await _dietPlanRepository.ActivatePlanAsync(planId);
                if (!success)
                {
                    return GenericResponse<string>.Fail(Messages.DietPlanNotFound, 404);
                }

                return GenericResponse<string>.Success(planId, Messages.DietPlanActivated);
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> DeactivateDietPlanAsync(string planId)
        {
            try
            {
                var success = await _dietPlanRepository.DeactivatePlanAsync(planId);
                if (!success)
                {
                    return GenericResponse<string>.Fail(Messages.DietPlanNotFound, 404);
                }

                return GenericResponse<string>.Success(planId, Messages.DietPlanDeactivated);
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<DietPlan>> CloneDietPlanAsync(string planId, string newClientId)
        {
            try
            {
                var originalPlan = await _dietPlanRepository.GetPlanWithMealsAsync(planId);
                if (originalPlan == null)
                {
                    return GenericResponse<DietPlan>.Fail(Messages.DietPlanNotFound, 404);
                }

                var client = await _userRepository.GetByIdAsync(newClientId);
                if (client == null || client.UserType != "Client")
                {
                    return GenericResponse<DietPlan>.Fail("Invalid client", 400);
                }

                var clonedPlan = new DietPlan
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = $"{originalPlan.Title} (Copy)",
                    Description = originalPlan.Description,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(originalPlan.DurationInDays - 1),
                    DailyCalorieTarget = originalPlan.DailyCalorieTarget,
                    PlanType = originalPlan.PlanType,
                    SpecialInstructions = originalPlan.SpecialInstructions,
                    ClientId = newClientId,
                    DietitianId = originalPlan.DietitianId,
                    CreationDate = DateTime.UtcNow,
                    Status = Status.Active
                };

                await _dietPlanRepository.AddAsync(clonedPlan);
                await _dietPlanRepository.SaveAsync();

                return GenericResponse<DietPlan>.Success(clonedPlan, "Diet plan cloned successfully");
            }
            catch (Exception ex)
            {
                return GenericResponse<DietPlan>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<Dictionary<string, decimal>>> GetDietPlanStatisticsAsync(string planId)
        {
            try
            {
                var dietPlan = await _dietPlanRepository.GetPlanWithMealsAsync(planId);
                if (dietPlan == null)
                {
                    return GenericResponse<Dictionary<string, decimal>>.Fail(Messages.DietPlanNotFound, 404);
                }

                var statistics = new Dictionary<string, decimal>
                {
                    ["TotalMeals"] = dietPlan.Meals.Count,
                    ["TotalCalories"] = dietPlan.Meals.Sum(m => m.Calories ?? 0),
                    ["TotalProtein"] = dietPlan.Meals.Sum(m => m.Protein ?? 0),
                    ["TotalCarbohydrates"] = dietPlan.Meals.Sum(m => m.Carbohydrates ?? 0),
                    ["TotalFat"] = dietPlan.Meals.Sum(m => m.Fat ?? 0),
                    ["AverageCaloriesPerMeal"] = dietPlan.Meals.Count > 0 ?
                        dietPlan.Meals.Sum(m => m.Calories ?? 0) / dietPlan.Meals.Count : 0,
                    ["DurationInDays"] = dietPlan.DurationInDays
                };

                return GenericResponse<Dictionary<string, decimal>>.Success(statistics, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<Dictionary<string, decimal>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        private async Task<GenericResponse<DietPlan>> ValidateDietPlanAsync(DietPlan dietPlan)
        {
            // Validate client exists and is a client
            var client = await _userRepository.GetByIdAsync(dietPlan.ClientId);
            if (client == null || client.UserType != "Client")
            {
                return GenericResponse<DietPlan>.Fail("Invalid client", 400);
            }

            // Validate dietitian exists and is a dietitian
            var dietitian = await _userRepository.GetByIdAsync(dietPlan.DietitianId);
            if (dietitian == null || dietitian.UserType != "Dietitian")
            {
                return GenericResponse<DietPlan>.Fail("Invalid dietitian", 400);
            }

            // Validate date range
            if (dietPlan.StartDate >= dietPlan.EndDate)
            {
                return GenericResponse<DietPlan>.Fail(Messages.InvalidDateRange, 400);
            }

            // Validate start date is not in the past (allow today)
            if (dietPlan.StartDate < DateTime.Today)
            {
                return GenericResponse<DietPlan>.Fail("Start date cannot be in the past", 400);
            }

            return GenericResponse<DietPlan>.Success(dietPlan, "Validation passed");
        }
    }
}
