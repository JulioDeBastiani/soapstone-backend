using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soapstone.Domain.Interfaces
{
    public interface IInterface<T> where T : Entity
    {
        Task<int> AddAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task<int> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetPage(int skip, int take);
        Task<IQueryable<T>> GetQueryable();
        Task<T> GetById(Guid id);

    }
}