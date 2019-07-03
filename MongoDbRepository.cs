using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        public async Task DeleteAsync<T, TKey>(Expression<Func<T, bool>> expression) where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            var collection = _db.GetCollection<T>(typeof(T).Name);
            await collection.DeleteManyAsync(expression);
        }

        public async Task DeleteAsync<T, TKey>(TKey id) where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            var collection = _db.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, id);
            await collection.DeleteOneAsync(filter);
        }

        public async Task DeleteAll<T, TKey>() where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            await _db.DropCollectionAsync(typeof(T).Name);
        }

        public async Task<T> SingleAsync<T, TKey>(Expression<Func<T, bool>> expression) where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            var collection = _db.GetCollection<T>(typeof(T).Name);
            return await collection.AsQueryable().Where(expression).SingleAsync();
        }

        public async Task<(int total, IList<T> results)> ListAsync<T, TKey>(int skip = 0, int take = 50) where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            var query = _db.GetCollection<T>(typeof(T).Name).AsQueryable();
            var total = query.CountAsync();
            var results = query.Skip(skip).Take(take).ToListAsync();
            await Task.WhenAll(total, results);
            return (total.Result, (IList<T>)results.Result);
        }
        
        public async Task<(int total, IList<T> results)> ListWithFilterAsync<T, TKey>(Expression<Func<T, bool>> filter, int skip = 0, int take = 50) where T : IDocument<TKey>  where TKey : IEquatable<TKey>
        {
            var query = _db.GetCollection<T>(typeof(T).Name).AsQueryable();
            query = query.Where(filter);
            var total = query.CountAsync();
            var results = query.Skip(skip).Take(take).ToListAsync();
            await Task.WhenAll(total, results);
            return (total.Result, (IList<T>)results.Result);
        }
        
        public async Task<T> SingleAsync<T, TKey>(TKey id) where T : IDocument<TKey>  where TKey : IEquatable<TKey> 
        {
            var collection = _db.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, id);
            return await collection.Find(filter).SingleAsync();
        }

        public async Task AddAsync<T, TKey>(T item) where T : IDocument<TKey> where TKey : IEquatable<TKey>
        {
            var def = default(TKey);
            if (item.Id == null || (def != null && def.Equals(item.Id)))
            {
                throw new Exception("You must provide a key to store this document.");
            }
            var collection = _db.GetCollection<T>(typeof(T).Name);
            await collection.InsertOneAsync(item);
        }

        public async Task AddAsync<T, TKey>(IEnumerable<T> items) where T : IDocument<TKey> where TKey : IEquatable<TKey>
        {
            foreach (var item in items)
            {
                var def = default(TKey);
                if (item.Id == null || (def != null && def.Equals(item.Id)))
                {
                    throw new Exception("You must provide a key to store this document.");
                }
            }

            var collection = _db.GetCollection<T>(typeof(T).Name);
            await collection.InsertManyAsync(items);
        }
        
    }
}