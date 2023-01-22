using MagicVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // based on some condition if you want to get result
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null);
        Task CreateAsync(T entity);
        
        //Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
    }
}
