using Microsoft.AspNetCore.Mvc;
using ElegantTrinkets2.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElegantTrinkets2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly IRepository<CartItem> _cartItemRepository;

        public CartItemsController(IRepository<CartItem> cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        // GET: api/cartitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems()
        {
            var result = await _cartItemRepository.GetAllAsync();
            return Ok(result);
        }

        // GET: api/cartitems/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItemsByUser(int userId)
        {
            var cartItems = (await _cartItemRepository.GetAllAsync())
                            .Where(ci => ci.UserId == userId)
                            .ToList();

            if (!cartItems.Any())
            {
                return NotFound();
            }

            return Ok(cartItems);
        }

        // POST: api/cartitems
        [HttpPost]
        public async Task<ActionResult<CartItem>> AddCartItem(CartItem cartItem)
        {
            await _cartItemRepository.AddAsync(cartItem);
            return CreatedAtAction(nameof(GetCartItems), new { id = cartItem.Id }, cartItem);
        }

        // PUT: api/cartitems/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, CartItem cartItem)
        {
            if (id != cartItem.Id)
            {
                return BadRequest();
            }

            try
            {
                await _cartItemRepository.UpdateAsync(cartItem);
            }
            catch
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/cartitems/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            // Pass the id instead of the whole CartItem object
            await _cartItemRepository.DeleteAsync(id);
            return NoContent();
        }

    }
}
