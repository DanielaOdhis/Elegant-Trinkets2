using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElegantTrinkets2.Data
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products.ToListAsync();
        public async Task<Product> GetByIdAsync(int id) => await _context.Products.FindAsync(id);
        public async Task AddAsync(Product product) => await _context.Products.AddAsync(product);
        public Task UpdateAsync(Product product) { _context.Products.Update(product); return Task.CompletedTask; }
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null) _context.Products.Remove(product);
        }
    }
}
