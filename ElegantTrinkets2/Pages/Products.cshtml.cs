using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElegantTrinkets2.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ElegantTrinkets2.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                Products = (await _unitOfWork.Products.GetAllAsync())
                            .Where(p => p.Name.Contains(searchQuery) || p.Description.Contains(searchQuery))
                            .ToList(); // Convert to List after filtering
            }
            else
            {
                // If no search query, show all products
                Products = (await _unitOfWork.Products.GetAllAsync()).ToList(); // Convert to List directly
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
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
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

            await _unitOfWork.CartItems.AddAsync(cartItem); // Add to cart using the repository
            await _unitOfWork.SaveAsync(); // Save changes through UnitOfWork

            return RedirectToPage("/Cart"); // Redirect to cart after adding
        }
    }
}
