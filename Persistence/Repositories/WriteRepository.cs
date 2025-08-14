using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Repositories;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T>
        where T : class, IBaseEntity
    {
        private readonly SHFTDbContext db;

        public WriteRepository(SHFTDbContext db)
        {
            this.db = db;
        }

        public DbSet<T> Table => db.Set<T>();

        public async Task<bool> AddAsync(T entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.CreationDate = DateTime.UtcNow;
            entity.Status = Domain.Enums.Status.Active;
            EntityEntry<T> entityEntry = await Table.AddAsync(entity);
            return entityEntry.State == EntityState.Added;
        }

        public async Task<bool> AddRangeAsync(List<T> entities)
        {
            await Table.AddRangeAsync(entities);
            return true;
        }

        public bool Remove(T entity)
        {
            entity.Status = Domain.Enums.Status.Pasive;
            entity.DeleteDate = DateTime.UtcNow;
            EntityEntry<T> entityEntry = Table.Update(entity);
            return entityEntry.State == EntityState.Modified;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            T model = await Table.FirstOrDefaultAsync(a => a.Id == id);
            return Remove(model);
        }

        public bool RemoveRange(List<T> entities)
        {
            Table.RemoveRange(entities);
            return true;
        }

        public bool Update(T entity)
        {
            entity.UpdateDate = DateTime.UtcNow;
            entity.Status = Domain.Enums.Status.Modified;
            EntityEntry<T> entityEntry = Table.Update(entity);
            return entityEntry.State == EntityState.Modified;
        }

        public Task<int> SaveAsync() => db.SaveChangesAsync();

        /// <summary>
        /// Hard delete
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            EntityEntry<T> entityEntry = Table.Remove(entity);
            return entityEntry.State == EntityState.Deleted;
        }
    }
}
