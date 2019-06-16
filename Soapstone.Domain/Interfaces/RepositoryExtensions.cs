using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soapstone.Domain.Defaults;

namespace Soapstone.Domain.Interfaces
{
    public static class ReposiotryExtensions
    {
        public static Task<IEnumerable<TEntity>> GetPageAsync<TEntity>(this IRepository<TEntity> repository, int skip = PaginationDefaults.DefaultSkip, int take = PaginationDefaults.DefaultTake) where TEntity : Entity
            => repository.GetPageAsync(null, null, null, skip, take);

        public static Task<IEnumerable<TEntity>> GetPageAsync<TEntity>(this IRepository<TEntity> repository, Func<TEntity, bool> predicate, int skip = PaginationDefaults.DefaultSkip, int take = PaginationDefaults.DefaultTake) where TEntity : Entity
            => repository.GetPageAsync(predicate, null, null, skip, take);

        public static Task<IEnumerable<TEntity>> GetPageAsync<TEntity>(this IRepository<TEntity> repository, Func<TEntity, object> orderBy, int skip = PaginationDefaults.DefaultSkip, int take = PaginationDefaults.DefaultTake) where TEntity : Entity
            => repository.GetPageAsync(null, orderBy, null, skip, take);

        public static Task<IEnumerable<TEntity>> GetPageAsync<TEntity>(this IRepository<TEntity> repository, Func<TEntity, bool> predicate, Func<TEntity, object> orderBy, int skip = PaginationDefaults.DefaultSkip, int take = PaginationDefaults.DefaultTake) where TEntity : Entity
            => repository.GetPageAsync(predicate, orderBy, null, skip, take);

        public static Task<TEntity> GetByIdAsync<TEntity>(this IRepository<TEntity> repository, Guid id) where TEntity : Entity
            => repository.GetByIdAsync(id, null);

        public static async Task<bool> ExistsAsync<TEntity>(this IRepository<TEntity> repository, Guid id) where TEntity : Entity
            => (await repository.GetByIdAsync(id)) != null;

        public static async Task<TEntity> SingleOrDefaultAsync<TEntity>(this IRepository<TEntity> repository, Func<TEntity, bool> predicate) where TEntity : Entity
            => (await repository.GetQueryableAsync()).SingleOrDefault(predicate);
    }
}