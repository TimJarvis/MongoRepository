using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoRepository
{
    /// <summary>
    /// Use this class if you would like to strongly type your access to a Mongo repository and would like
    /// a standard mechanism to apply a filter and / or data restrictions to your Document.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TCriteria"></typeparam>
    public abstract class DocumentManager<T, TKey, TCriteria> : IDocumentManager<T, TKey, TCriteria> where T : IDocument<TKey> where TKey : IEquatable<TKey>
    {
        private readonly IRepository _repository;
        
        private DocumentManager(){}
        
        protected DocumentManager(IRepository repository)
        {
            _repository = repository;
        }
        
        public virtual Task DeleteAsync(Expression<Func<T, bool>> expression)
        {
            return _repository.DeleteAsync<T, TKey>(expression);
        }

        public virtual Task DeleteAsync(TKey id)
        {
            return _repository.DeleteAsync<T, TKey>(id);
        }

        public virtual Task DeleteAllAsync()
        {
            return _repository.DeleteAll<T, TKey>();
        }

        public virtual Task<T> SingleAsync(Expression<Func<T, bool>> expression)
        {
            return _repository.SingleAsync<T, TKey>(expression);
        }

        public virtual Task<T> SingleAsync(TKey id)
        {
            return _repository.SingleAsync<T, TKey>(id);
        }

        /// <summary>
        /// Use this method to convert a criteria object to a filter (expression tree - predicate)
        /// Ideal to use a specification pattern or something like that here.
        /// If you do not require a filter, simply return null.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected abstract Expression<Func<T, bool>> BuildFilter(TCriteria criteria);

        /// <summary>
        /// Use this method to apply data restrictions to your Document, here is
        /// where you will restrict to a specific tenant (for example)
        /// usually you would inject a helper service to perform this for you.
        /// If you do not require any additional security simply return null.
        /// </summary>
        /// <returns></returns>
        protected abstract Expression<Func<T, bool>> ApplyDataSecurity();

        public virtual Task<(int total, IList<T> results)> ListAsync(TCriteria criteria, int skip = 0, int take = 50)
        {
            var security = ApplyDataSecurity() ?? ((T) => true);
            var filter = BuildFilter(criteria) ?? ((T) => true);
            BinaryExpression and = Expression.AndAlso(security.Body, filter.Body);
            var predicate = Expression.Lambda<Func<T, bool>>(and, security.Parameters.Single());
            return _repository.ListWithFilterAsync<T, TKey>(predicate, skip, take);
        }

        public virtual Task AddAsync(T item)
        {
            return _repository.AddAsync<T, TKey>(item);
        }

        public Task AddAsync(IEnumerable<T> items)
        {
            return _repository.AddAsync<T, TKey>(items);
        }
    }
}