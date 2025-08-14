using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser?> GetByUserNameAsync(string userName);
        Task<IList<ApplicationUser>> GetUsersByRoleAsync(string roleName);
        Task<IList<ApplicationUser>> GetDietitiansAsync();
        Task<IList<ApplicationUser>> GetClientsAsync();
        Task<IList<ApplicationUser>> GetClientsByDietitianAsync(string dietitianId);
        Task<List<ApplicationUser>> GetUsersAsync(Expression<Func<ApplicationUser, bool>> predicate);
        Task<bool> CreateAsync(ApplicationUser user, string password);
        Task<bool> UpdateAsync(ApplicationUser user);
        Task<bool> DeleteAsync(string id);
        Task<bool> AssignClientToDietitianAsync(string clientId, string dietitianId);
        Task<bool> RemoveClientFromDietitianAsync(string clientId);
    }
}
