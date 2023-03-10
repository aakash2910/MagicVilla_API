using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;
        internal DbSet<T> dbSet;
        public Repository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            this.dbSet= _dbContext.Set<T>();
        }

        public async Task CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
