using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoRepository
{
    public interface IRepository
    {
        Task DeleteAsync<T, TKey>(Expression<Func<T, bool>> expression) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task DeleteAsync<T, TKey>(TKey id) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task DeleteAll<T, TKey>() where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task<T> SingleAsync<T, TKey>(Expression<Func<T, bool>> expression) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task<T> SingleAsync<T, TKey>(TKey id) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task<(int total, IList<T> results)> ListAsync<T, TKey>(int skip = 0, int take = 50) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task<(int total, IList<T> results)> ListWithFilterAsync<T, TKey>(Expression<Func<T,bool>> filter, int skip = 0, int take = 50) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task AddAsync<T, TKey>(T item) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task AddAsync<T, TKey>(IEnumerable<T> items) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
    }
}