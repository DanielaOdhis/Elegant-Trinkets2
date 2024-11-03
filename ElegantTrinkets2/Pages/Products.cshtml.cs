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

        public List<Product> Products { get; set; }

        public async Task OnGetAsync()
        {
            Products = await _context.Products.ToListAsync();
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

            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = 1 // Default quantity
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Cart"); // Redirect to cart after adding
        }
    }
    }
