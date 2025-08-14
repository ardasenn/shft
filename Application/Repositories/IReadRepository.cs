using System.Linq.Expressions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories
{
    public interface IReadRepository<T> : IRepository<T>
        where T : class, IBaseEntity
    {
        IQueryable<T> GetAll(bool asNoTracking = false);
        IQueryable<T> GetWhere(Expression<Func<T, bool>> expression, bool asNoTracking = false);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> expression);
        Task<T> GetByIdAsync(string id, params Expression<Func<T, object>>[] includes);
        Task<List<T>> GetAllAsync(bool asNoTracking = false);
        Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> expression, bool asNoTracking = false);
    }
}
