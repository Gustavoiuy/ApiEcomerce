using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IRepository
{
    /// <summary>
    /// Interfaz genérica para operaciones CRUD asíncronas.
    /// </summary>
    /// <typeparam name="T">Entidad</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
