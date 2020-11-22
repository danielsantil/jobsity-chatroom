using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace JobsityChatroom.WebAPI.Data.Repository
{
    public class Repository <T> : IRepository<T> where T : EntityBase
    {
        private readonly ChatroomDbContext _context;

        public Repository(ChatroomDbContext context)
        {
            _context = context;
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAll(Expression<Func<T, object>> orderBy)
        {
            return await _context.Set<T>()
                .OrderBy(orderBy)
                .ToListAsync();
        }

        public virtual async Task<T> Get(Expression<Func<T, bool>> expression)
        {
            var entity = await _context.Set<T>().Where(expression).FirstOrDefaultAsync();
            _context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public virtual async Task<int> Count(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().CountAsync(expression);
        }

        public virtual async Task Insert(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public virtual async Task Delete(Expression<Func<T, bool>> expression)
        {
            var entity = await Get(expression);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
