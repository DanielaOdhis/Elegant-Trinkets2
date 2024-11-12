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
        private readonly IUnitOfWork _unitOfWork;

        // Constructor for dependency injection with IUnitOfWork
        public CartModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<CartItemView> CartItems { get; set; } = new List<CartItemView>();
        public double TotalPrice { get; set; } = 0;

        public async Task OnGetAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var cartItems = await _unitOfWork.CartItems.GetAllAsync();
            cartItems = cartItems.Where(c => c.UserId == userId);

            foreach (var cartItem in cartItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                if (product != null)
                {
                    CartItems.Add(new CartItemView
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = cartItem.Quantity,
                        ImageUrl = product.ImageUrl
                    });
                    TotalPrice += product.Price * cartItem.Quantity;
                }
            }
        }

        public async Task<IActionResult> OnPostCheckoutAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cartItems = await _unitOfWork.CartItems.GetAllAsync();
            cartItems = cartItems.Where(c => c.UserId == userId).ToList();

            // Logic for checkout

            // Clear the cart after checkout
            foreach (var cartItem in cartItems)
            {
                await _unitOfWork.CartItems.DeleteAsync(cartItem.Id);
            }
            await _unitOfWork.SaveAsync();

            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostRemoveAsync(int productId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cartItems = await _unitOfWork.CartItems.GetAllAsync();
            var cartItem = cartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                await _unitOfWork.CartItems.DeleteAsync(cartItem.Id);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync(int productId, int quantity)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cartItems = await _unitOfWork.CartItems.GetAllAsync();
            var cartItem = cartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null && quantity > 0)
            {
                cartItem.Quantity = quantity;
                await _unitOfWork.SaveAsync();
            }

            return RedirectToPage();
        }

        public class CartItemView
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
            public string ImageUrl { get; set; }
        }
    }
}
