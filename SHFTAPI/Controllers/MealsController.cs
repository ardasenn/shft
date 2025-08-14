using Application.Services;
using Application.Utilities.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SHFTAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MealsController : BaseController
    {
        private readonly IMealService _mealService;
        private readonly IDietPlanService _dietPlanService;

        public MealsController(IMealService mealService, IDietPlanService dietPlanService)
        {
            _mealService = mealService;
            _dietPlanService = dietPlanService;
        }

        /// <summary>
        /// Get all meals (Admin only)
        /// </summary>
        /// <returns>List of all meals</returns>
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetAllMeals()
        {
            var result = await _mealService.GetAllMealsAsync();
            return CreateResponse(result);
        }

        /// <summary>
        /// Get meal by ID
        /// </summary>
        /// <param name="id">Meal ID</param>
        /// <returns>Meal information</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMealById(string id)
        {
            var result = await _mealService.GetMealByIdAsync(id);

            // Check authorization - users can only view meals from their associated plans
            if (result.IsSuccess && result.Data != null)
            {
                var planResult = await _dietPlanService.GetDietPlanByIdAsync(result.Data.DietPlanId);
                if (planResult.IsSuccess && planResult.Data != null)
                {
                    var currentUserId = GetCurrentUserId();
                    if (!IsAdmin() &&
                        planResult.Data.ClientId != currentUserId &&
                        planResult.Data.DietitianId != currentUserId)
                    {
                        return Forbid();
                    }
                }
            }

            return CreateResponse(result);
        }

        /// <summary>
        /// Get meals by diet plan ID
        /// </summary>
        /// <param name="dietPlanId">Diet plan ID</param>
        /// <returns>List of meals for the diet plan</returns>
        [HttpGet("diet-plan/{dietPlanId}")]
        public async Task<IActionResult> GetMealsByDietPlan(string dietPlanId)
        {
            // Check authorization for the diet plan
            var planResult = await _dietPlanService.GetDietPlanByIdAsync(dietPlanId);
            if (!planResult.IsSuccess || planResult.Data == null)
            {
                return CreateResponse(planResult);
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() &&
                planResult.Data.ClientId != currentUserId &&
                planResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _mealService.GetMealsByDietPlanAsync(dietPlanId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get meals by meal type
        /// </summary>
        /// <param name="mealType">Meal type (Breakfast, Lunch, Dinner, Snack)</param>
        /// <returns>List of meals of specified type</returns>
        [HttpGet("type/{mealType}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> GetMealsByType(string mealType)
        {
            var result = await _mealService.GetMealsByTypeAsync(mealType);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get meals for a specific date
        /// </summary>
        /// <param name="dietPlanId">Diet plan ID</param>
        /// <param name="date">Date</param>
        /// <returns>List of meals for the specified date</returns>
        [HttpGet("diet-plan/{dietPlanId}/date/{date}")]
        public async Task<IActionResult> GetMealsForDate(string dietPlanId, DateTime date)
        {
            // Check authorization for the diet plan
            var planResult = await _dietPlanService.GetDietPlanByIdAsync(dietPlanId);
            if (!planResult.IsSuccess || planResult.Data == null)
            {
                return CreateResponse(planResult);
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() &&
                planResult.Data.ClientId != currentUserId &&
                planResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _mealService.GetMealsForDateAsync(dietPlanId, date);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get meals by time range
        /// </summary>
        /// <param name="dietPlanId">Diet plan ID</param>
        /// <param name="startTime">Start time (HH:mm format)</param>
        /// <param name="endTime">End time (HH:mm format)</param>
        /// <returns>List of meals within the time range</returns>
        [HttpGet("diet-plan/{dietPlanId}/time-range")]
        public async Task<IActionResult> GetMealsByTimeRange(string dietPlanId, [FromQuery] string startTime, [FromQuery] string endTime)
        {
            // Check authorization for the diet plan
            var planResult = await _dietPlanService.GetDietPlanByIdAsync(dietPlanId);
            if (!planResult.IsSuccess || planResult.Data == null)
            {
                return CreateResponse(planResult);
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() &&
                planResult.Data.ClientId != currentUserId &&
                planResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            // Parse time strings
            if (!TimeSpan.TryParse(startTime, out var startTimeSpan) ||
                !TimeSpan.TryParse(endTime, out var endTimeSpan))
            {
                return BadRequest("Invalid time format. Use HH:mm format.");
            }

            var result = await _mealService.GetMealsByTimeRangeAsync(dietPlanId, startTimeSpan, endTimeSpan);
            return CreateResponse(result);
        }

        /// <summary>
        /// Create a new meal
        /// </summary>
        /// <param name="meal">Meal information</param>
        /// <returns>Created meal</returns>
        [HttpPost]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> CreateMeal([FromBody] Meal meal)
        {
            // Check authorization for the diet plan
            var planResult = await _dietPlanService.GetDietPlanByIdAsync(meal.DietPlanId);
            if (!planResult.IsSuccess || planResult.Data == null)
            {
                return CreateResponse(planResult);
            }

            // Dietitians can only create meals for their own plans
            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && planResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _mealService.CreateMealAsync(meal);
            return CreateResponse(result);
        }

        /// <summary>
        /// Update meal
        /// </summary>
        /// <param name="id">Meal ID</param>
        /// <param name="meal">Updated meal information</param>
        /// <returns>Updated meal</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> UpdateMeal(string id, [FromBody] Meal meal)
        {
            // First get the existing meal to check ownership
            var existingMealResult = await _mealService.GetMealByIdAsync(id);
            if (!existingMealResult.IsSuccess || existingMealResult.Data == null)
            {
                return CreateResponse(existingMealResult);
            }

            // Check authorization for the associated diet plan
            var planResult = await _dietPlanService.GetDietPlanByIdAsync(existingMealResult.Data.DietPlanId);
            if (!planResult.IsSuccess || planResult.Data == null)
            {
                return CreateResponse(planResult);
            }

            // Dietitians can only update meals from their own plans
            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && planResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            meal.Id = id; // Ensure the ID matches
            var result = await _mealService.UpdateMealAsync(meal);
            return CreateResponse(result);
        }

        /// <summary>
        /// Delete meal
        /// </summary>
        /// <param name="id">Meal ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> DeleteMeal(string id)
        {
            // First get the existing meal to check ownership
            var existingMealResult = await _mealService.GetMealByIdAsync(id);
            if (!existingMealResult.IsSuccess || existingMealResult.Data == null)
            {
                return CreateResponse(existingMealResult);
            }

            // Check authorization for the associated diet plan
            var planResult = await _dietPlanService.GetDietPlanByIdAsync(existingMealResult.Data.DietPlanId);
            if (!planResult.IsSuccess || planResult.Data == null)
            {
                return CreateResponse(planResult);
            }

            // Dietitians can only delete meals from their own plans
            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && planResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _mealService.DeleteMealAsync(id);
            return CreateResponse(result);
        }

        /// <summary>
        /// Create multiple meals for a diet plan
        /// </summary>
        /// <param name="dietPlanId">Diet plan ID</param>
        /// <param name="meals">List of meals</param>
        /// <returns>Created meals</returns>
        [HttpPost("diet-plan/{dietPlanId}/bulk")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> CreateMealPlan(string dietPlanId, [FromBody] List<Meal> meals)
        {
            // Check authorization for the diet plan
            var planResult = await _dietPlanService.GetDietPlanByIdAsync(dietPlanId);
            if (!planResult.IsSuccess || planResult.Data == null)
            {
                return CreateResponse(planResult);
            }

            // Dietitians can only create meals for their own plans
            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && planResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _mealService.CreateMealPlanAsync(dietPlanId, meals);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get meal nutrition summary
        /// </summary>
        /// <param name="mealId">Meal ID</param>
        /// <returns>Nutrition summary for the meal</returns>
        [HttpGet("{mealId}/nutrition")]
        public async Task<IActionResult> GetMealNutritionSummary(string mealId)
        {
            // Check authorization
            var mealResult = await _mealService.GetMealByIdAsync(mealId);
            if (!mealResult.IsSuccess || mealResult.Data == null)
            {
                return CreateResponse(mealResult);
            }

            var planResult = await _dietPlanService.GetDietPlanByIdAsync(mealResult.Data.DietPlanId);
            if (planResult.IsSuccess && planResult.Data != null)
            {
                var currentUserId = GetCurrentUserId();
                if (!IsAdmin() &&
                    planResult.Data.ClientId != currentUserId &&
                    planResult.Data.DietitianId != currentUserId)
                {
                    return Forbid();
                }
            }

            var result = await _mealService.GetMealNutritionSummaryAsync(mealId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get daily nutrition summary for a diet plan
        /// </summary>
        /// <param name="dietPlanId">Diet plan ID</param>
        /// <param name="date">Date</param>
        /// <returns>Daily nutrition summary</returns>
        [HttpGet("diet-plan/{dietPlanId}/nutrition/daily/{date}")]
        public async Task<IActionResult> GetDailyNutritionSummary(string dietPlanId, DateTime date)
        {
            // Check authorization for the diet plan
            var planResult = await _dietPlanService.GetDietPlanByIdAsync(dietPlanId);
            if (!planResult.IsSuccess || planResult.Data == null)
            {
                return CreateResponse(planResult);
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() &&
                planResult.Data.ClientId != currentUserId &&
                planResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _mealService.GetDailyNutritionSummaryAsync(dietPlanId, date);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get total calories for a diet plan
        /// </summary>
        /// <param name="dietPlanId">Diet plan ID</param>
        /// <returns>Total calories for the diet plan</returns>
        [HttpGet("diet-plan/{dietPlanId}/calories/total")]
        public async Task<IActionResult> GetTotalCaloriesForPlan(string dietPlanId)
        {
            // Check authorization for the diet plan
            var planResult = await _dietPlanService.GetDietPlanByIdAsync(dietPlanId);
            if (!planResult.IsSuccess || planResult.Data == null)
            {
                return CreateResponse(planResult);
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() &&
                planResult.Data.ClientId != currentUserId &&
                planResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _mealService.GetTotalCaloriesForPlanAsync(dietPlanId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get total calories for a specific date
        /// </summary>
        /// <param name="dietPlanId">Diet plan ID</param>
        /// <param name="date">Date</param>
        /// <returns>Total calories for the specified date</returns>
        [HttpGet("diet-plan/{dietPlanId}/calories/date/{date}")]
        public async Task<IActionResult> GetTotalCaloriesForDate(string dietPlanId, DateTime date)
        {
            // Check authorization for the diet plan
            var planResult = await _dietPlanService.GetDietPlanByIdAsync(dietPlanId);
            if (!planResult.IsSuccess || planResult.Data == null)
            {
                return CreateResponse(planResult);
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() &&
                planResult.Data.ClientId != currentUserId &&
                planResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _mealService.GetTotalCaloriesForDateAsync(dietPlanId, date);
            return CreateResponse(result);
        }
    }
}
