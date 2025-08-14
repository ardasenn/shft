using Application.Services;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            // Business Logic Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IApplicationUserService, ApplicationUserService>();
            services.AddScoped<IDietPlanService, DietPlanService>();
            services.AddScoped<IMealService, MealService>();

            // Additional infrastructure services can be added here
            // For example: Email services, File services, External API services, etc.
        }
    }
}