using System.Linq.Expressions;
using Application.Repositories;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class ReadRepository<T> : IReadRepository<T>
        where T : class, IBaseEntity
    {
        private readonly SHFTDbContext db;

        public ReadRepository(SHFTDbContext db)
        {
            this.db = db;
        }

        public DbSet<T> Table => db.Set<T>();

        public IQueryable<T> GetAll(bool asNoTracking = false)
        {
            var query = asNoTracking ? Table.AsNoTracking() : Table;
            return query
                .Where(a => a.Status != Domain.Enums.Status.Pasive)
                .OrderByDescending(a => a.CreationDate);
        }

        public async Task<T> GetByIdAsync(string id, params Expression<Func<T, object>>[] includes)
        {
            var query = Table.Where(a => a.Id == id && a.Status != Domain.Enums.Status.Pasive);

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync(bool asNoTracking = false)
        {
            var query = asNoTracking ? Table.AsNoTracking() : Table;
            return await query
                .Where(a => a.Status != Domain.Enums.Status.Pasive)
                .OrderByDescending(a => a.CreationDate)
                .ToListAsync();
        }

        public async Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> expression, bool asNoTracking = false)
        {
            var query = asNoTracking ? Table.AsNoTracking() : Table;
            return await query
                .Where(a => a.Status != Domain.Enums.Status.Pasive)
                .Where(expression)
                .OrderByDescending(a => a.CreationDate)
                .ToListAsync();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> expression) =>
            await Table
                .Where(a => a.Status != Domain.Enums.Status.Pasive)
                .FirstOrDefaultAsync(expression);

        public IQueryable<T> GetWhere(
            Expression<Func<T, bool>> expression,
            bool asNoTracking = false
        )
        {
            var query = asNoTracking ? Table.AsNoTracking() : Table;
            return query
                .Where(a => a.Status != Domain.Enums.Status.Pasive)
                .Where(expression)
                .OrderByDescending(a => a.CreationDate);
        }
    }
}
