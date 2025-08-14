using Application.Utilities.Response;
using Domain.Entities;

namespace Application.Services
{
    public interface IApplicationUserService
    {
        Task<GenericResponse<List<ApplicationUser>>> GetAllUsersAsync();
        Task<GenericResponse<ApplicationUser>> GetUserByIdAsync(string id);
        Task<GenericResponse<ApplicationUser>> GetUserByEmailAsync(string email);
        Task<GenericResponse<List<ApplicationUser>>> GetUsersByRoleAsync(string roleName);
        Task<GenericResponse<List<ApplicationUser>>> GetDietitiansAsync();
        Task<GenericResponse<List<ApplicationUser>>> GetClientsAsync();
        Task<GenericResponse<List<ApplicationUser>>> GetClientsByDietitianAsync(string dietitianId);
        Task<GenericResponse<ApplicationUser>> CreateUserAsync(ApplicationUser user, string password);
        Task<GenericResponse<ApplicationUser>> UpdateUserAsync(ApplicationUser user);
        Task<GenericResponse<string>> DeleteUserAsync(string id);
        Task<GenericResponse<string>> AssignClientToDietitianAsync(string clientId, string dietitianId);
        Task<GenericResponse<string>> RemoveClientFromDietitianAsync(string clientId);
        Task<GenericResponse<string>> AddUserToRoleAsync(string userId, string roleName);
        Task<GenericResponse<string>> RemoveUserFromRoleAsync(string userId, string roleName);
        Task<GenericResponse<List<string>>> GetUserRolesAsync(string userId);
    }
}
