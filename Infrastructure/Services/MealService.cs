using Application.Repositories;
using Application.Services;
using Application.Utilities.Constants;
using Application.Utilities.Response;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Services
{
    public class MealService : IMealService
    {
        private readonly IMealRepository _mealRepository;
        private readonly IDietPlanRepository _dietPlanRepository;

        public MealService(
            IMealRepository mealRepository,
            IDietPlanRepository dietPlanRepository)
        {
            _mealRepository = mealRepository;
            _dietPlanRepository = dietPlanRepository;
        }

        public async Task<GenericResponse<List<Meal>>> GetAllMealsAsync()
        {
            try
            {
                var meals = await _mealRepository.GetAllAsync();
                return GenericResponse<List<Meal>>.Success(meals, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<Meal>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<Meal>> GetMealByIdAsync(string id)
        {
            try
            {
                var meal = await _mealRepository.GetByIdAsync(id);
                if (meal == null)
                {
                    return GenericResponse<Meal>.Fail(Messages.MealNotFound, 404);
                }

                return GenericResponse<Meal>.Success(meal, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<Meal>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<Meal>>> GetMealsByDietPlanAsync(string dietPlanId)
        {
            try
            {
                var meals = await _mealRepository.GetMealsByDietPlanAsync(dietPlanId);
                return GenericResponse<List<Meal>>.Success(meals.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<Meal>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<Meal>>> GetMealsByTypeAsync(string mealType)
        {
            try
            {
                var meals = await _mealRepository.GetMealsByTypeAsync(mealType);
                return GenericResponse<List<Meal>>.Success(meals.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<Meal>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<Meal>>> GetMealsForDateAsync(string dietPlanId, DateTime date)
        {
            try
            {
                var meals = await _mealRepository.GetMealsForDateAsync(dietPlanId, date);
                return GenericResponse<List<Meal>>.Success(meals.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<Meal>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<Meal>>> GetMealsByTimeRangeAsync(string dietPlanId, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                var meals = await _mealRepository.GetMealsByTimeRangeAsync(dietPlanId, startTime, endTime);
                return GenericResponse<List<Meal>>.Success(meals.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<Meal>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<Meal>> CreateMealAsync(Meal meal)
        {
            try
            {
                // Validate diet plan exists
                var dietPlan = await _dietPlanRepository.GetByIdAsync(meal.DietPlanId);
                if (dietPlan == null)
                {
                    return GenericResponse<Meal>.Fail(Messages.DietPlanNotFound, 404);
                }

                // Validate business rules
                var validationResult = ValidateMeal(meal);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                await _mealRepository.AddAsync(meal);
                await _mealRepository.SaveAsync();

                return GenericResponse<Meal>.Success(meal, Messages.MealCreated);
            }
            catch (Exception ex)
            {
                return GenericResponse<Meal>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<Meal>> UpdateMealAsync(Meal meal)
        {
            try
            {
                var existingMeal = await _mealRepository.GetByIdAsync(meal.Id);
                if (existingMeal == null)
                {
                    return GenericResponse<Meal>.Fail(Messages.MealNotFound, 404);
                }

                // Validate business rules
                var validationResult = ValidateMeal(meal);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }

                _mealRepository.Update(meal);
                await _mealRepository.SaveAsync();

                return GenericResponse<Meal>.Success(meal, Messages.MealUpdated);
            }
            catch (Exception ex)
            {
                return GenericResponse<Meal>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> DeleteMealAsync(string id)
        {
            try
            {
                var meal = await _mealRepository.GetByIdAsync(id);
                if (meal == null)
                {
                    return GenericResponse<string>.Fail(Messages.MealNotFound, 404);
                }

                await _mealRepository.RemoveAsync(id);
                await _mealRepository.SaveAsync();

                return GenericResponse<string>.Success(id, Messages.MealDeleted);
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<Meal>>> CreateMealPlanAsync(string dietPlanId, List<Meal> meals)
        {
            try
            {
                var dietPlan = await _dietPlanRepository.GetByIdAsync(dietPlanId);
                if (dietPlan == null)
                {
                    return GenericResponse<List<Meal>>.Fail(Messages.DietPlanNotFound, 404);
                }

                // Validate all meals
                foreach (var meal in meals)
                {
                    meal.DietPlanId = dietPlanId;
                    var validationResult = ValidateMeal(meal);
                    if (!validationResult.IsSuccess)
                    {
                        return GenericResponse<List<Meal>>.Fail(validationResult.Message, validationResult.StatusCode);
                    }
                }

                await _mealRepository.AddRangeAsync(meals);
                await _mealRepository.SaveAsync();

                return GenericResponse<List<Meal>>.Success(meals, "Meal plan created successfully");
            }
            catch (Exception ex)
            {
                return GenericResponse<List<Meal>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<Dictionary<string, decimal>>> GetMealNutritionSummaryAsync(string mealId)
        {
            try
            {
                var meal = await _mealRepository.GetByIdAsync(mealId);
                if (meal == null)
                {
                    return GenericResponse<Dictionary<string, decimal>>.Fail(Messages.MealNotFound, 404);
                }

                var summary = new Dictionary<string, decimal>
                {
                    ["Calories"] = meal.Calories ?? 0,
                    ["Protein"] = meal.Protein ?? 0,
                    ["Carbohydrates"] = meal.Carbohydrates ?? 0,
                    ["Fat"] = meal.Fat ?? 0,
                    ["Fiber"] = meal.Fiber ?? 0,
                    ["Sugar"] = meal.Sugar ?? 0,
                    ["Sodium"] = meal.Sodium ?? 0,
                    ["CaloriesPerServing"] = meal.CaloriesPerServing ?? 0
                };

                return GenericResponse<Dictionary<string, decimal>>.Success(summary, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<Dictionary<string, decimal>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<Dictionary<string, decimal>>> GetDailyNutritionSummaryAsync(string dietPlanId, DateTime date)
        {
            try
            {
                var meals = await _mealRepository.GetMealsForDateAsync(dietPlanId, date);
                
                var summary = new Dictionary<string, decimal>
                {
                    ["TotalMeals"] = meals.Count,
                    ["TotalCalories"] = meals.Sum(m => m.Calories ?? 0),
                    ["TotalProtein"] = meals.Sum(m => m.Protein ?? 0),
                    ["TotalCarbohydrates"] = meals.Sum(m => m.Carbohydrates ?? 0),
                    ["TotalFat"] = meals.Sum(m => m.Fat ?? 0),
                    ["TotalFiber"] = meals.Sum(m => m.Fiber ?? 0),
                    ["TotalSugar"] = meals.Sum(m => m.Sugar ?? 0),
                    ["TotalSodium"] = meals.Sum(m => m.Sodium ?? 0)
                };

                return GenericResponse<Dictionary<string, decimal>>.Success(summary, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<Dictionary<string, decimal>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<decimal>> GetTotalCaloriesForPlanAsync(string dietPlanId)
        {
            try
            {
                var totalCalories = await _mealRepository.GetTotalCaloriesForPlanAsync(dietPlanId);
                return GenericResponse<decimal>.Success(totalCalories ?? 0, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<decimal>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<decimal>> GetTotalCaloriesForDateAsync(string dietPlanId, DateTime date)
        {
            try
            {
                var totalCalories = await _mealRepository.GetTotalCaloriesForDateAsync(dietPlanId, date);
                return GenericResponse<decimal>.Success(totalCalories ?? 0, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<decimal>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        private GenericResponse<Meal> ValidateMeal(Meal meal)
        {
            if (string.IsNullOrWhiteSpace(meal.Title))
            {
                return GenericResponse<Meal>.Fail("Meal title is required", 400);
            }

            if (string.IsNullOrWhiteSpace(meal.MealType))
            {
                return GenericResponse<Meal>.Fail("Meal type is required", 400);
            }

            if (meal.Calories < 0 || meal.Protein < 0 || meal.Carbohydrates < 0 || meal.Fat < 0)
            {
                return GenericResponse<Meal>.Fail("Nutritional values cannot be negative", 400);
            }

            if (meal.ServingSize <= 0)
            {
                return GenericResponse<Meal>.Fail("Serving size must be greater than zero", 400);
            }

            if (meal.PreparationTimeMinutes < 0)
            {
                return GenericResponse<Meal>.Fail("Preparation time cannot be negative", 400);
            }

            return GenericResponse<Meal>.Success(meal, "Validation passed");
        }
    }
}
