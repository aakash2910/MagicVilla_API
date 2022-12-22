using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IVillaRepository
    {
        // based on some condition if you want to get result
        Task<List<Villa>> GetAll(Expression<Func<Villa, bool>> filter = null);
        Task<Villa> Get(Expression<Func<Villa, bool>> filter = null);
        Task Create(Villa entity);
        Task Remove(Villa entity);
        //Task Save();
    }
}
