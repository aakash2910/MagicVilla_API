using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public class VillaRepository : IVillaRepository
    {
        private readonly AppDbContext _dbContext;
        public VillaRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(Villa entity)
        {
            await _dbContext.Villas.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Villa> Get(Expression<Func<Villa, bool>> filter = null)
        {
            IQueryable<Villa> query = _dbContext.Villas;

            if (filter != null)
                query = query.Where(filter);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Villa>> GetAll(Expression<Func<Villa,bool>> filter = null)
        {
            IQueryable<Villa> query = _dbContext.Villas;
            
            if(filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }

        public async Task Remove(Villa entity)
        {
            _dbContext.Remove(entity);
            _dbContext.SaveChangesAsync();
        }
    }
}
