using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoRepository
{
    public interface IQuery<TResult, in TCriteria>
    {
        Task<(int total, IList<TResult> results)> Execute(TCriteria criteria, int skip = 0, int take = 50);
    }
}