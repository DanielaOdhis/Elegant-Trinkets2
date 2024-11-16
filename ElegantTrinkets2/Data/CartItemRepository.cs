using ElegantTrinkets2.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class CartItemRepository : IRepository<CartItem>
{
    private readonly ApplicationDbContext _context;

    public CartItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CartItem>> GetAllAsync()
    {
        return await _context.CartItems.ToListAsync();
    }

    public async Task<CartItem> GetByIdAsync(int id)
    {
        return await _context.CartItems.FindAsync(id);
    }

    public async Task AddAsync(CartItem entity)
    {
        _context.CartItems.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CartItem entity)
    {
        _context.CartItems.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.CartItems.FindAsync(id);
        if (entity != null)
        {
            _context.CartItems.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
