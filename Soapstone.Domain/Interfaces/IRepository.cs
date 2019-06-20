using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Soapstone.Domain.Defaults;

namespace Soapstone.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<int> AddAsync(TEntity entity);
        Task<int> UpdateAsync(TEntity entity);
        Task<int> DeleteAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetPageAsync(Func<TEntity, bool> predicate, Func<TEntity, object> orderBy, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes, int skip = PaginationDefaults.DefaultSkip, int take = PaginationDefaults.DefaultTake);
        Task<IEnumerable<TEntity>> GetPageDescendingAsync(Func<TEntity, bool> predicate, Func<TEntity, object> orderBy, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes, int skip = PaginationDefaults.DefaultSkip, int take = PaginationDefaults.DefaultTake);
        Task<IQueryable<TEntity>> GetQueryableAsync();
        Task<TEntity> GetByIdAsync(Guid id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes);

    }
}