using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        private readonly AppDbContext _dbContext;
        public VillaRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }       

        public async Task<Villa> UpdateAsync(Villa entity)
        {
            entity.UpdatedDate= DateTime.UtcNow;
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
