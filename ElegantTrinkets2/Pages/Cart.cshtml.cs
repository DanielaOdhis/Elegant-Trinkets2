using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElegantTrinkets2.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ElegantTrinkets2.Pages
{
    [Authorize]
    public class CartModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CartModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<CartItemView> CartItems { get; set; } = new List<CartItemView>();
        public double TotalPrice { get; set; } = 0;

        public async Task OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle unauthenticated user (e.g., redirect to login)
                // You could also choose to return or throw an exception if appropriate.
                return; // or RedirectToPage("/Account/Login");
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                // Handle case where the user ID claim is missing
                return; // or RedirectToPage("/Account/Login");
            }

            // Convert the user ID to int
            var userId = int.Parse(userIdClaim);

            // Fetch cart items and their corresponding product details
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            foreach (var cartItem in cartItems)
            {
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product != null)
                {
                    CartItems.Add(new CartItemView
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = cartItem.Quantity
                    });
                    // Update total price
                    TotalPrice += product.Price * cartItem.Quantity;
                }
            }
        }

        public async Task<IActionResult> OnPostCheckoutAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Get the user ID as an int

            // Logic for checkout

            // Clear the cart after checkout
            var cartItems = _context.CartItems.Where(c => c.UserId == userId);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index"); // Redirect after checkout
        }

        public async Task<IActionResult> OnPostRemoveAsync(int productId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Get the user ID as an int
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(); // Refresh the cart page
        }

        public async Task<IActionResult> OnPostUpdateAsync(int productId, int quantity)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null && quantity > 0)
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(); // Refresh the cart page
        }

        public class CartItemView
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
        }
    }
}
