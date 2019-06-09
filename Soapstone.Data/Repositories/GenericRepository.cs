using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Soapstone.Domain;
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

        public async Task<int> UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            // TODO undeletable entities
            _context.Set<TEntity>().Remove(entity);
            return await _context.SaveChangesAsync();
        }

        // TODO Extract defaults
        // TODO skip/take consistency
        // TODO predicate/include if needed
        public Task<IEnumerable<TEntity>> GetPage(int skip = 0, int take = 25)
        {
            return Task.FromResult(_context.Set<TEntity>().Skip(skip).Take(take).AsEnumerable());
        }

        public Task<IQueryable<TEntity>> GetQueryable()
        {
            return Task.FromResult(_context.Set<TEntity>().AsQueryable());
        }

        // TODO include if needed
        public async Task<TEntity> GetById(Guid id)
        {
            return await _context.Set<TEntity>().SingleOrDefaultAsync(e => e.Id == id);
        }
    }
}