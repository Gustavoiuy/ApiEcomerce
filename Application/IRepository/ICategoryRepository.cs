
using Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Application.IRepository
{
    /// <summary>
    /// Repositorio específico para Category, hereda de IGenericRepository.
    /// Agrega métodos adicionales si son necesarios.
    /// </summary>
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> CategoryExistsAsync(int id);
        Task<bool> CategoryExistsAsync(string name);
        // Agrega aquí métodos específicos de Category si los necesitas
    }
}
