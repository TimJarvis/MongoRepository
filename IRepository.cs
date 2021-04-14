using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoRepository
{
    public interface IRepository
    {
        Task DeleteAsync<T, TKey>(Expression<Func<T, bool>> expression, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task DeleteAsync<T, TKey>(TKey id, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task DeleteAll<T, TKey>(string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task<T> SingleAsync<T, TKey>(Expression<Func<T, bool>> expression, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task<T> SingleAsync<T, TKey>(TKey id, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task<T> SingleOrDefaultAsync<T, TKey>(TKey id, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task<(int total, IList<T> results)> ListAsync<T, TKey>(int skip = 0, int take = 50, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task<(int total, IList<T> results)> ListAsync<T, TKey>( string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task<(int total, IList<T> results)> ListWithFilterAsync<T, TKey>(Expression<Func<T,bool>> filter, int skip = 0, int take = 50, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task<(int total, IList<T> results)> ListWithFilterAsync<T, TKey>(Expression<Func<T,bool>> filter, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task AddAsync<T, TKey>(T item, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task AddAsync<T, TKey>(IEnumerable<T> items, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
        Task UpsertAsync<T, TKey>(T item, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>;
    }
}