using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoRepository
{
    
    public abstract class QueryBase<TResult, TCriteria> : IQuery<TResult, TCriteria> 
    {
        private readonly IMongoDatabase _db;

        private QueryBase() { }

        protected  QueryBase(IMongoDatabase db)
        {
            _db = db;
        }

        protected IMongoQueryable<TEntity> GetQuery<TEntity>() 
        {
            return _db.GetCollection<TEntity>(typeof(TEntity).Name).AsQueryable();
        }

        protected IMongoQueryable<TEntity> GetQueryWithFilter<TEntity>(Expression<Func<TEntity,bool>> filter) 
        {
            var query = _db.GetCollection<TEntity>(typeof(TEntity).Name).AsQueryable();
            return query.Where(filter);
        }

        protected Expression<Func<TEntity, bool>> AndExpressions<TEntity>(Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right)
        {
            BinaryExpression and = Expression.AndAlso(left.Body, right.Body);
            return Expression.Lambda<Func<TEntity, bool>>(and, left.Parameters.Single());   
        }
        
        protected Expression<Func<TEntity, bool>> OrExpressions<TEntity>(Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right)
        {
            BinaryExpression or = Expression.OrElse(left.Body, right.Body);
            return Expression.Lambda<Func<TEntity, bool>>(or, left.Parameters.Single());   
        }

        public abstract Task<(int total, IList<TResult> results)> Execute(TCriteria criteria, int skip = 0, int take = 50);
    }
}