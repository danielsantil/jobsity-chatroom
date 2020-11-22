using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JobsityChatroom.WebAPI.Data.Repository
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression,
            Expression<Func<T, object>> orderBy, int limit);
        Task<T> Get(Expression<Func<T, bool>> expression);
        Task<int> Count(Expression<Func<T, bool>> expression);
        Task Insert(T entity);
        Task Update(T entity);
        Task Delete(Expression<Func<T, bool>> expression);
    }
}
