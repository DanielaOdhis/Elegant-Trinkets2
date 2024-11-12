using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElegantTrinkets2.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ElegantTrinkets2.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProductsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Properties
        public List<Product> Products { get; set; }
        public string SearchQuery { get; set; }  // Added property for search query

        public async Task OnGetAsync(string searchQuery)
        {
            SearchQuery = searchQuery;  // Capture the search query from the URL

            // If searchQuery is provided, filter products based on the query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                Products = await _context.Products
                                          .Where(p => p.Name.Contains(searchQuery) || p.Description.Contains(searchQuery))
                                          .ToListAsync();
            }
            else
            {
                // If no search query, show all products
                Products = await _context.Products.ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int productId)
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login"); // Redirect to login page if not authenticated
            }

            // Retrieve the UserId from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToPage("/Account/Login"); // Redirect to login page if userId claim is not found
            }

            var userId = int.Parse(userIdClaim.Value); // Convert string to int

            // Check if the product exists
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound(); // Handle case where the product is not found
            }

            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                ImageUrl = product.ImageUrl,
                Quantity = 1 // Default quantity
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Cart"); // Redirect to cart after adding
        }
    }
}
