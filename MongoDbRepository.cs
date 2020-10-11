using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoRepository
{
    public class MongoDbRepository : IRepository
    {
        private readonly IMongoDatabase _db;

        private MongoDbRepository() { }

        public MongoDbRepository(IMongoDatabase db)
        {
            _db = db;
        }

        public async Task DeleteAsync<T, TKey>(Expression<Func<T, bool>> expression, string collectionName = null) where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            var derivedCollectionName = typeof(T).Name;
            var collection = _db.GetCollection<T>(collectionName ?? derivedCollectionName);
            await collection.DeleteManyAsync(expression);
        }

        public async Task DeleteAsync<T, TKey>(TKey id, string collectionName = null) where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            var derivedCollectionName = typeof(T).Name;
            var collection = _db.GetCollection<T>(collectionName ?? derivedCollectionName);
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, id);
            await collection.DeleteOneAsync(filter);
        }

        public async Task DeleteAll<T, TKey>(string collectionName = null) where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            var derivedCollectionName = typeof(T).Name;
            await _db.DropCollectionAsync(collectionName ?? derivedCollectionName);
        }

        public async Task<T> SingleAsync<T, TKey>(Expression<Func<T, bool>> expression, string collectionName = null) where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            var derivedCollectionName = typeof(T).Name;
            var collection = _db.GetCollection<T>(collectionName ?? derivedCollectionName);
            return await collection.AsQueryable().Where(expression).SingleAsync();
        }

        public async Task<(int total, IList<T> results)> ListAsync<T, TKey>(int skip = 0, int take = 50, string collectionName = null) where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            var derivedCollectionName = typeof(T).Name;
            var query = _db.GetCollection<T>(collectionName ?? derivedCollectionName).AsQueryable();
            var total = query.CountAsync();
            var results = query.Skip(skip).Take(take).ToListAsync();
            await Task.WhenAll(total, results);
            return (total.Result, (IList<T>)results.Result);
        }

        public Task<(int total, IList<T> results)> ListAsync<T, TKey>(string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>
        {
            return ListAsync<T,TKey>(0, 50, collectionName);
        }

        public async Task<(int total, IList<T> results)> ListWithFilterAsync<T, TKey>(Expression<Func<T, bool>> filter, int skip = 0, int take = 50, string collectionName = null) where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            var derivedCollectionName = typeof(T).Name;
            var query = _db.GetCollection<T>(collectionName ?? derivedCollectionName).AsQueryable();
            query = query.Where(filter);
            var total = query.CountAsync();
            var results = query.Skip(skip).Take(take).ToListAsync();
            await Task.WhenAll(total, results);
            return (total.Result, (IList<T>)results.Result);
        }

        public  Task<(int total, IList<T> results)> ListWithFilterAsync<T, TKey>(Expression<Func<T, bool>> filter, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>
        {
            return ListWithFilterAsync<T, TKey>(filter, 0, 50, collectionName);
        }
        
        public async Task<T> SingleAsync<T, TKey>(TKey id, string collectionName = null) where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            var derivedCollectionName = typeof(T).Name;
            var collection = _db.GetCollection<T>(collectionName ?? derivedCollectionName);
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, id);
            return await collection.Find(filter).SingleAsync();
        }

        public async Task AddAsync<T, TKey>(T item, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>
        {
            var derivedCollectionName = typeof(T).Name;
            var def = default(TKey);
            // if the id is null or equals the default value for that type - then throw.
            if (item.Id == null || (def != null && def.Equals(item.Id)))
            {
                throw new Exception("You must provide a key to store this document.");
            }
            var collection = _db.GetCollection<T>(collectionName ?? derivedCollectionName);
            await collection.InsertOneAsync(item);
        }
        
        public async Task UpsertAsync<T, TKey>(T item, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>
        {
            var derivedCollectionName = typeof(T).Name;
            var def = default(TKey);
            // if the id is null or equals the default value for that type - then throw.
            if (item.Id == null || (def != null && def.Equals(item.Id)))
            {
                throw new Exception("You must provide a key to store this document.");
            }
            var collection = _db.GetCollection<T>(collectionName ?? derivedCollectionName);
            await collection.ReplaceOneAsync(filter: f => f.Id.Equals(item.Id) , options: new ReplaceOptions() {IsUpsert = true}, replacement: item);
        }

        public async Task AddAsync<T, TKey>(IEnumerable<T> items, string collectionName = null) where T : IDocument<TKey> where TKey : IEquatable<TKey>
        {
            var derivedCollectionName = typeof(T).Name;
            foreach (var item in items)
            {
                var def = default(TKey);
                if (item.Id == null || (def != null && def.Equals(item.Id)))
                {
                    throw new Exception("You must provide a key to store this document.");
                }
            }

            var collection = _db.GetCollection<T>(collectionName ?? derivedCollectionName);
            await collection.InsertManyAsync(items);
        }
    }
}