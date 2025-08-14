using Application.Repositories;
using Application.Services;
using Application.Utilities.Constants;
using Application.Utilities.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationUserService(
            IApplicationUserRepository userRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<GenericResponse<List<ApplicationUser>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetUsersAsync(u => u.Status != Domain.Enums.Status.Pasive);
                return GenericResponse<List<ApplicationUser>>.Success(users, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<ApplicationUser>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<ApplicationUser>> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return GenericResponse<ApplicationUser>.Fail(Messages.UserNotFound, 404);
                }

                return GenericResponse<ApplicationUser>.Success(user, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<ApplicationUser>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<ApplicationUser>> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return GenericResponse<ApplicationUser>.Fail(Messages.UserNotFound, 404);
                }

                return GenericResponse<ApplicationUser>.Success(user, Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<ApplicationUser>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<ApplicationUser>>> GetUsersByRoleAsync(string roleName)
        {
            try
            {
                var users = await _userRepository.GetUsersByRoleAsync(roleName);
                return GenericResponse<List<ApplicationUser>>.Success(users.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<ApplicationUser>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<ApplicationUser>>> GetDietitiansAsync()
        {
            try
            {
                var dietitians = await _userRepository.GetDietitiansAsync();
                return GenericResponse<List<ApplicationUser>>.Success(dietitians.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<ApplicationUser>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<ApplicationUser>>> GetClientsAsync()
        {
            try
            {
                var clients = await _userRepository.GetClientsAsync();
                return GenericResponse<List<ApplicationUser>>.Success(clients.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<ApplicationUser>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<ApplicationUser>>> GetClientsByDietitianAsync(string dietitianId)
        {
            try
            {
                var clients = await _userRepository.GetClientsByDietitianAsync(dietitianId);
                return GenericResponse<List<ApplicationUser>>.Success(clients.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<ApplicationUser>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<ApplicationUser>> CreateUserAsync(ApplicationUser user, string password)
        {
            try
            {
                // Validate business rules
                if (user.UserType == "Client" && !string.IsNullOrEmpty(user.DietitianId))
                {
                    var dietitian = await _userRepository.GetByIdAsync(user.DietitianId);
                    if (dietitian == null || dietitian.UserType != "Dietitian")
                    {
                        return GenericResponse<ApplicationUser>.Fail("Invalid dietitian assignment", 400);
                    }
                }

                var success = await _userRepository.CreateAsync(user, password);
                if (!success)
                {
                    return GenericResponse<ApplicationUser>.Fail(Messages.SaveFail, 400);
                }

                return GenericResponse<ApplicationUser>.Success(user, Messages.UserCreated);
            }
            catch (Exception ex)
            {
                return GenericResponse<ApplicationUser>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<ApplicationUser>> UpdateUserAsync(ApplicationUser user)
        {
            try
            {
                // Validate business rules
                if (user.UserType == "Client" && !string.IsNullOrEmpty(user.DietitianId))
                {
                    var dietitian = await _userRepository.GetByIdAsync(user.DietitianId);
                    if (dietitian == null || dietitian.UserType != "Dietitian")
                    {
                        return GenericResponse<ApplicationUser>.Fail("Invalid dietitian assignment", 400);
                    }
                }

                var success = await _userRepository.UpdateAsync(user);
                if (!success)
                {
                    return GenericResponse<ApplicationUser>.Fail(Messages.SaveFail, 400);
                }

                return GenericResponse<ApplicationUser>.Success(user, Messages.UserUpdated);
            }
            catch (Exception ex)
            {
                return GenericResponse<ApplicationUser>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> DeleteUserAsync(string id)
        {
            try
            {
                var success = await _userRepository.DeleteAsync(id);
                if (!success)
                {
                    return GenericResponse<string>.Fail(Messages.UserNotFound, 404);
                }

                return GenericResponse<string>.Success(id, Messages.UserDeleted);
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> AssignClientToDietitianAsync(string clientId, string dietitianId)
        {
            try
            {
                var client = await _userRepository.GetByIdAsync(clientId);
                var dietitian = await _userRepository.GetByIdAsync(dietitianId);

                if (client == null || client.UserType != "Client")
                {
                    return GenericResponse<string>.Fail("Invalid client", 400);
                }

                if (dietitian == null || dietitian.UserType != "Dietitian")
                {
                    return GenericResponse<string>.Fail("Invalid dietitian", 400);
                }

                var success = await _userRepository.AssignClientToDietitianAsync(clientId, dietitianId);
                if (!success)
                {
                    return GenericResponse<string>.Fail(Messages.SaveFail, 400);
                }

                return GenericResponse<string>.Success("Assignment successful", Messages.UserAssignedToDietitian);
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> RemoveClientFromDietitianAsync(string clientId)
        {
            try
            {
                var success = await _userRepository.RemoveClientFromDietitianAsync(clientId);
                if (!success)
                {
                    return GenericResponse<string>.Fail("Failed to remove client from dietitian", 400);
                }

                return GenericResponse<string>.Success("Removal successful", Messages.UserRemovedFromDietitian);
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> AddUserToRoleAsync(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return GenericResponse<string>.Fail(Messages.UserNotFound, 404);
                }

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    return GenericResponse<string>.Fail("Role does not exist", 400);
                }

                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return GenericResponse<string>.Fail($"Failed to add role: {errors}", 400);
                }

                return GenericResponse<string>.Success("Role added successfully", Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return GenericResponse<string>.Fail(Messages.UserNotFound, 404);
                }

                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return GenericResponse<string>.Fail($"Failed to remove role: {errors}", 400);
                }

                return GenericResponse<string>.Success("Role removed successfully", Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<List<string>>> GetUserRolesAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return GenericResponse<List<string>>.Fail(Messages.UserNotFound, 404);
                }

                var roles = await _userManager.GetRolesAsync(user);
                return GenericResponse<List<string>>.Success(roles.ToList(), Messages.Success);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<string>>.Fail($"{Messages.Fail}: {ex.Message}", 500);
            }
        }
    }
}
