using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories
{
    public interface IRepository<T>
        where T : class, IBaseEntity
    {
        DbSet<T> Table { get; }
    }
}
