using System;
using Domain.Entities;
using Application.IRepository;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
namespace Infrastructure.Repository;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _db;
    public ProductRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<bool> BuyProductAsync(string name, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
            return false;
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
        if (product == null || product.Stock < quantity)
            return false;
        product.Stock -= quantity;
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Product>> GetProductsInPagesAsync(int pageNumber, int pageSize)
    {
        return await _db.Products.OrderBy(p => p.ProductId)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<int> GetTotalProductsAsync()
    {
        return await _db.Products.CountAsync();
    }

    public async Task<ICollection<Product>> GetProductsForCategoryAsync(int categoryId)
    {
        if (categoryId <= 0)
            return new List<Product>();
        return await _db.Products.Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId).OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<ICollection<Product>> SearchProductsAsync(string searchTerm)
    {
        IQueryable<Product> query = _db.Products;
        var searchTermLowered = searchTerm.ToLower().Trim();
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Include(p => p.Category).Where(
                p => p.Name.ToLower().Trim().Contains(searchTermLowered) ||
                     p.Description.ToLower().Trim().Contains(searchTermLowered));
        }
        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<bool> ProductExistsAsync(int id)
    {
        if (id <= 0)
            return false;
        return await _db.Products.AnyAsync(p => p.ProductId == id);
    }

    public async Task<bool> ProductExistsAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        return await _db.Products.AnyAsync(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
    }
}
