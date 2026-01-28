using Domain.Entities;
using Application.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Infrastructure.Data;
namespace Infrastructure.Repository
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _db.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> CategoryExistsAsync(string name)
        {
            return await _db.Categories.AnyAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
        }
    }
}
