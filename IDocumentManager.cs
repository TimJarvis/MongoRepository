using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoRepository
{
    public interface IDocumentManager<T, TKey, TCriteria> where T : IDocument<TKey> where TKey : IEquatable<TKey>
    {
        Task DeleteAsync(Expression<Func<T, bool>> expression);
        Task DeleteAsync(TKey id);
        Task DeleteAllAsync();
        Task<T> SingleAsync(Expression<Func<T, bool>> expression);
        Task<T> SingleAsync(TKey id);
        Task<(int total, IList<T> results)> ListAsync(TCriteria criteria, int skip = 0, int take = 50);
        Task AddAsync(T item);
        Task AddAsync(IEnumerable<T> items);
    }
}