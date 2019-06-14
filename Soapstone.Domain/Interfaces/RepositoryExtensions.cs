using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soapstone.Domain.Interfaces
{
    public static class ReposiotryExtensions
    {
        public static Task<IEnumerable<TEntity>> GetPageAsync<TEntity>(this IRepository<TEntity> repository, int skip = 0, int take = 25) where TEntity : Entity
            => repository.GetPageAsync(skip, take);
    }
}