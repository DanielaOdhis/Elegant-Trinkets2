using ElegantTrinkets2.Pages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElegantTrinkets2.Data
{
    public class CartItemRepository : IRepository<CartItem>
    {
        private readonly ApplicationDbContext _context;

        public CartItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItem>> GetAllAsync() => await _context.CartItems.ToListAsync();
        public async Task<CartItem> GetByIdAsync(int id) => await _context.CartItems.FindAsync(id);
        public async Task AddAsync(CartItem cartItem) => await _context.CartItems.AddAsync(cartItem);
        public Task UpdateAsync(CartItem cartItem) { _context.CartItems.Update(cartItem); return Task.CompletedTask; }
        public async Task DeleteAsync(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null) _context.CartItems.Remove(cartItem);
        }
    }
}
