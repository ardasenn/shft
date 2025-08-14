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
    public class DietPlansController : BaseController
    {
        private readonly IDietPlanService _dietPlanService;

        public DietPlansController(IDietPlanService dietPlanService)
        {
            _dietPlanService = dietPlanService;
        }

        /// <summary>
        /// Get all diet plans (Admin only)
        /// </summary>
        /// <returns>List of all diet plans</returns>
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetAllDietPlans()
        {
            var result = await _dietPlanService.GetAllDietPlansAsync();
            return CreateResponse(result);
        }

        /// <summary>
        /// Get diet plan by ID
        /// </summary>
        /// <param name="id">Diet plan ID</param>
        /// <returns>Diet plan information</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDietPlanById(string id)
        {
            var result = await _dietPlanService.GetDietPlanByIdAsync(id);

            // Check authorization - users can only view plans they're associated with
            if (result.IsSuccess && result.Data != null)
            {
                var currentUserId = GetCurrentUserId();
                if (!IsAdmin() &&
                    result.Data.ClientId != currentUserId &&
                    result.Data.DietitianId != currentUserId)
                {
                    return Forbid();
                }
            }

            return CreateResponse(result);
        }

        /// <summary>
        /// Get diet plans by client ID
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <returns>List of diet plans for the client</returns>
        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetDietPlansByClient(string clientId)
        {
            // Authorization check
            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && clientId != currentUserId && !IsDietitian())
            {
                return Forbid();
            }

            var result = await _dietPlanService.GetDietPlansByClientAsync(clientId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get diet plans by dietitian ID
        /// </summary>
        /// <param name="dietitianId">Dietitian ID</param>
        /// <returns>List of diet plans created by the dietitian</returns>
        [HttpGet("dietitian/{dietitianId}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> GetDietPlansByDietitian(string dietitianId)
        {
            // Dietitians can only view their own plans
            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && dietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _dietPlanService.GetDietPlansByDietitianAsync(dietitianId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get active diet plans
        /// </summary>
        /// <returns>List of active diet plans</returns>
        [HttpGet("active")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> GetActiveDietPlans()
        {
            var result = await _dietPlanService.GetActiveDietPlansAsync();
            return CreateResponse(result);
        }

        /// <summary>
        /// Get diet plans ending in specified days
        /// </summary>
        /// <param name="days">Number of days</param>
        /// <returns>List of diet plans ending in specified days</returns>
        [HttpGet("ending-in/{days}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> GetDietPlansEndingInDays(int days)
        {
            var result = await _dietPlanService.GetDietPlansEndingInDaysAsync(days);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get diet plan with meals
        /// </summary>
        /// <param name="planId">Diet plan ID</param>
        /// <returns>Diet plan with all meals</returns>
        [HttpGet("{planId}/meals")]
        public async Task<IActionResult> GetDietPlanWithMeals(string planId)
        {
            var result = await _dietPlanService.GetDietPlanWithMealsAsync(planId);

            // Check authorization
            if (result.IsSuccess && result.Data != null)
            {
                var currentUserId = GetCurrentUserId();
                if (!IsAdmin() &&
                    result.Data.ClientId != currentUserId &&
                    result.Data.DietitianId != currentUserId)
                {
                    return Forbid();
                }
            }

            return CreateResponse(result);
        }

        /// <summary>
        /// Create a new diet plan
        /// </summary>
        /// <param name="dietPlan">Diet plan information</param>
        /// <returns>Created diet plan</returns>
        [HttpPost]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> CreateDietPlan([FromBody] DietPlan dietPlan)
        {
            // Dietitians can only create plans for themselves
            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && IsDietitian())
            {
                dietPlan.DietitianId = currentUserId;
            }

            var result = await _dietPlanService.CreateDietPlanAsync(dietPlan);
            return CreateResponse(result);
        }

        /// <summary>
        /// Update diet plan
        /// </summary>
        /// <param name="id">Diet plan ID</param>
        /// <param name="dietPlan">Updated diet plan information</param>
        /// <returns>Updated diet plan</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> UpdateDietPlan(string id, [FromBody] DietPlan dietPlan)
        {
            // First get the existing plan to check ownership
            var existingPlanResult = await _dietPlanService.GetDietPlanByIdAsync(id);
            if (!existingPlanResult.IsSuccess || existingPlanResult.Data == null)
            {
                return CreateResponse(existingPlanResult);
            }

            // Authorization check - dietitians can only update their own plans
            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && existingPlanResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            dietPlan.Id = id; // Ensure the ID matches
            var result = await _dietPlanService.UpdateDietPlanAsync(dietPlan);
            return CreateResponse(result);
        }

        /// <summary>
        /// Delete diet plan
        /// </summary>
        /// <param name="id">Diet plan ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> DeleteDietPlan(string id)
        {
            // First get the existing plan to check ownership
            var existingPlanResult = await _dietPlanService.GetDietPlanByIdAsync(id);
            if (!existingPlanResult.IsSuccess || existingPlanResult.Data == null)
            {
                return CreateResponse(existingPlanResult);
            }

            // Authorization check - dietitians can only delete their own plans
            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && existingPlanResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _dietPlanService.DeleteDietPlanAsync(id);
            return CreateResponse(result);
        }

        /// <summary>
        /// Activate diet plan
        /// </summary>
        /// <param name="planId">Diet plan ID</param>
        /// <returns>Success message</returns>
        [HttpPost("{planId}/activate")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> ActivateDietPlan(string planId)
        {
            // Check ownership
            var existingPlanResult = await _dietPlanService.GetDietPlanByIdAsync(planId);
            if (!existingPlanResult.IsSuccess || existingPlanResult.Data == null)
            {
                return CreateResponse(existingPlanResult);
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && existingPlanResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _dietPlanService.ActivateDietPlanAsync(planId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Deactivate diet plan
        /// </summary>
        /// <param name="planId">Diet plan ID</param>
        /// <returns>Success message</returns>
        [HttpPost("{planId}/deactivate")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> DeactivateDietPlan(string planId)
        {
            // Check ownership
            var existingPlanResult = await _dietPlanService.GetDietPlanByIdAsync(planId);
            if (!existingPlanResult.IsSuccess || existingPlanResult.Data == null)
            {
                return CreateResponse(existingPlanResult);
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && existingPlanResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _dietPlanService.DeactivateDietPlanAsync(planId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Clone diet plan for a new client
        /// </summary>
        /// <param name="planId">Original plan ID</param>
        /// <param name="newClientId">New client ID</param>
        /// <returns>Cloned diet plan</returns>
        [HttpPost("{planId}/clone")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> CloneDietPlan(string planId, [FromQuery] string newClientId)
        {
            // Check ownership of original plan
            var existingPlanResult = await _dietPlanService.GetDietPlanByIdAsync(planId);
            if (!existingPlanResult.IsSuccess || existingPlanResult.Data == null)
            {
                return CreateResponse(existingPlanResult);
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && existingPlanResult.Data.DietitianId != currentUserId)
            {
                return Forbid();
            }

            var result = await _dietPlanService.CloneDietPlanAsync(planId, newClientId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get diet plan statistics
        /// </summary>
        /// <param name="planId">Diet plan ID</param>
        /// <returns>Diet plan statistics</returns>
        [HttpGet("{planId}/statistics")]
        public async Task<IActionResult> GetDietPlanStatistics(string planId)
        {
            // Check authorization first
            var planResult = await _dietPlanService.GetDietPlanByIdAsync(planId);
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

            var result = await _dietPlanService.GetDietPlanStatisticsAsync(planId);
            return CreateResponse(result);
        }
    }
}
