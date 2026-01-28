
using Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Application.IRepository
{
    /// <summary>
    /// Repositorio específico para Product, hereda de IGenericRepository.
    /// Agrega métodos adicionales si son necesarios.
    /// </summary>
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<ICollection<Product>> GetProductsInPagesAsync(int pageNumber, int pageSize);
        Task<int> GetTotalProductsAsync();
        Task<ICollection<Product>> GetProductsForCategoryAsync(int categoryId);
        Task<ICollection<Product>> SearchProductsAsync(string searchTerm);
        Task<bool> BuyProductAsync(string name, int quantity);
        Task<bool> ProductExistsAsync(int id);
        Task<bool> ProductExistsAsync(string name);
        // Agrega aquí métodos específicos de Product si los necesitas
    }
}


