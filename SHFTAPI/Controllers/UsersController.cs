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
    public class UsersController : BaseController
    {
        private readonly IApplicationUserService _userService;

        public UsersController(IApplicationUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        /// <returns>List of all users</returns>
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return CreateResponse(result);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User information</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            // Users can only view their own profile unless they are admin or dietitian viewing their clients
            var currentUserId = GetCurrentUserId();
            if (id != currentUserId && !IsAdmin() && !IsDietitian())
            {
                return Forbid();
            }

            var result = await _userService.GetUserByIdAsync(id);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get users by role (Admin only)
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>List of users with specified role</returns>
        [HttpGet("role/{roleName}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            var result = await _userService.GetUsersByRoleAsync(roleName);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get all dietitians
        /// </summary>
        /// <returns>List of dietitians</returns>
        [HttpGet("dietitians")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Client}")]
        public async Task<IActionResult> GetDietitians()
        {
            var result = await _userService.GetDietitiansAsync();
            return CreateResponse(result);
        }

        /// <summary>
        /// Get all clients (Admin and Dietitian only)
        /// </summary>
        /// <returns>List of clients</returns>
        [HttpGet("clients")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> GetClients()
        {
            var result = await _userService.GetClientsAsync();
            return CreateResponse(result);
        }

        /// <summary>
        /// Get clients assigned to a specific dietitian
        /// </summary>
        /// <param name="dietitianId">Dietitian ID</param>
        /// <returns>List of clients assigned to the dietitian</returns>
        [HttpGet("dietitian/{dietitianId}/clients")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> GetClientsByDietitian(string dietitianId)
        {
            // Dietitians can only view their own clients
            var currentUserId = GetCurrentUserId();
            if (dietitianId != currentUserId && !IsAdmin())
            {
                return Forbid();
            }

            var result = await _userService.GetClientsByDietitianAsync(dietitianId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Create a new user (Admin only)
        /// </summary>
        /// <param name="user">User information</param>
        /// <param name="password">User password</param>
        /// <returns>Created user</returns>
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateUser([FromBody] ApplicationUser user, [FromQuery] string password)
        {
            var result = await _userService.CreateUserAsync(user, password);
            return CreateResponse(result);
        }

        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="user">Updated user information</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUser user)
        {
            // Users can only update their own profile unless they are admin
            var currentUserId = GetCurrentUserId();
            if (id != currentUserId && !IsAdmin())
            {
                return Forbid();
            }

            user.Id = id; // Ensure the ID matches
            var result = await _userService.UpdateUserAsync(user);
            return CreateResponse(result);
        }

        /// <summary>
        /// Delete user (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return CreateResponse(result);
        }

        /// <summary>
        /// Assign client to dietitian
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="dietitianId">Dietitian ID</param>
        /// <returns>Success message</returns>
        [HttpPost("assign-client")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> AssignClientToDietitian([FromQuery] string clientId, [FromQuery] string dietitianId)
        {
            // Dietitians can only assign clients to themselves
            var currentUserId = GetCurrentUserId();
            if (dietitianId != currentUserId && !IsAdmin())
            {
                return Forbid();
            }

            var result = await _userService.AssignClientToDietitianAsync(clientId, dietitianId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Remove client from dietitian
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <returns>Success message</returns>
        [HttpPost("remove-client")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Dietitian}")]
        public async Task<IActionResult> RemoveClientFromDietitian([FromQuery] string clientId)
        {
            var result = await _userService.RemoveClientFromDietitianAsync(clientId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Add user to role (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="roleName">Role name</param>
        /// <returns>Success message</returns>
        [HttpPost("{userId}/roles/{roleName}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AddUserToRole(string userId, string roleName)
        {
            var result = await _userService.AddUserToRoleAsync(userId, roleName);
            return CreateResponse(result);
        }

        /// <summary>
        /// Remove user from role (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="roleName">Role name</param>
        /// <returns>Success message</returns>
        [HttpDelete("{userId}/roles/{roleName}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            var result = await _userService.RemoveUserFromRoleAsync(userId, roleName);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get user roles
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user roles</returns>
        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            // Users can only view their own roles unless they are admin
            var currentUserId = GetCurrentUserId();
            if (userId != currentUserId && !IsAdmin())
            {
                return Forbid();
            }

            var result = await _userService.GetUserRolesAsync(userId);
            return CreateResponse(result);
        }
    }
}
