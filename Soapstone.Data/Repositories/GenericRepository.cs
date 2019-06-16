using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Soapstone.Domain;
using Soapstone.Domain.Defaults;
using Soapstone.Domain.Interfaces;

namespace Soapstone.Data
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            return _context.SaveChangesAsync();
        }

        public Task<int> DeleteAsync(TEntity entity)
        {
            // TODO undeletable entities
            _context.Set<TEntity>().Remove(entity);
            return _context.SaveChangesAsync();
        }

        // TODO include if needed
        public Task<IEnumerable<TEntity>> GetPageAsync(Func<TEntity, bool> predicate, Func<TEntity, object> orderBy, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes, int skip = PaginationDefaults.DefaultSkip, int take = PaginationDefaults.DefaultTake)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            skip = skip < 0 ? 0 : skip;
            take = take < 0 ? 0 : take;
            take = take > PaginationDefaults.DefaultMaxTake ? PaginationDefaults.DefaultMaxTake : take;

            if (predicate != null)
                query = query.Where(predicate).AsQueryable();

            if (includes != null)
                query = includes(query);

            if (orderBy != null)
                query = query.OrderBy(orderBy).AsQueryable();

            return Task.FromResult(_context.Set<TEntity>().Skip(skip).Take(take).AsEnumerable());
        }

        public Task<IQueryable<TEntity>> GetQueryableAsync()
        {
            return Task.FromResult(_context.Set<TEntity>().AsQueryable());
        }

        public Task<TEntity> GetByIdAsync(Guid id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes)
        {
            var queryable = _context.Set<TEntity>().AsQueryable();

            if (includes != null)
                queryable = includes(queryable);

            return queryable.SingleOrDefaultAsync(e => e.Id == id);
        }
    }
}