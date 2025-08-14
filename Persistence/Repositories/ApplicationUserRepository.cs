using System.Linq.Expressions;
using Application.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SHFTDbContext _context;

        public ApplicationUserRepository(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SHFTDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> GetByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<IList<ApplicationUser>> GetUsersByRoleAsync(string roleName)
        {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }

        public async Task<IList<ApplicationUser>> GetDietitiansAsync()
        {
            return await _userManager.GetUsersInRoleAsync("Dietitian");
        }

        public async Task<IList<ApplicationUser>> GetClientsAsync()
        {
            return await _userManager.GetUsersInRoleAsync("Client");
        }

        public async Task<IList<ApplicationUser>> GetClientsByDietitianAsync(string dietitianId)
        {
            return await _context
                .Users
                .Where(u => u.DietitianId == dietitianId && u.UserType == "Client")
                .ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetUsersAsync(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return await _context.Users
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Assign role based on UserType
                await _userManager.AddToRoleAsync(user, user.UserType);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(ApplicationUser user)
        {
            user.UpdateDate = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Soft delete
                user.DeleteDate = DateTime.UtcNow;
                user.Status = Domain.Enums.Status.Pasive;
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            return false;
        }

        public async Task<bool> AssignClientToDietitianAsync(string clientId, string dietitianId)
        {
            var client = await _userManager.FindByIdAsync(clientId);
            var dietitian = await _userManager.FindByIdAsync(dietitianId);

            if (client != null
                && dietitian != null
                && client.UserType == "Client"
                && dietitian.UserType == "Dietitian")
            {
                client.DietitianId = dietitianId;
                client.UpdateDate = DateTime.UtcNow;
                var result = await _userManager.UpdateAsync(client);
                return result.Succeeded;
            }
            return false;
        }

        public async Task<bool> RemoveClientFromDietitianAsync(string clientId)
        {
            var client = await _userManager.FindByIdAsync(clientId);
            if (client != null && client.UserType == "Client")
            {
                client.DietitianId = null;
                client.UpdateDate = DateTime.UtcNow;
                var result = await _userManager.UpdateAsync(client);
                return result.Succeeded;
            }
            return false;
        }
    }
}
