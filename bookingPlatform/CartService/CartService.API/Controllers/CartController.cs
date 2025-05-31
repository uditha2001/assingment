using CartService.API.DTO;
using CartService.API.services.interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartService.API.Controllers
{
    [ApiController]
    [Route("api/v1/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Gets the cart for a specific user.tis will retriew all items
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The user's cart items.</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartByUserId(long userId)
        {
            try
            {
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null)
                    return NotFound($"Cart not found for user id: {userId}");

                return Ok(cart);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving the cart.");
            }
        }

        /// <summary>
        /// Adds an item to the user's cart.
        /// </summary>
        /// <param name="item">The cart item to add.</param>
        /// <returns>The updated cart item.</returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddItemToCart([FromBody] CartItemDTO item)
        {
            try
            {
                if (item == null)
                    return BadRequest("Cart item cannot be null.");

                var updatedCartItem = await _cartService.AddItemToCartAsync(item);
                if (updatedCartItem)
                {
                    return Ok(updatedCartItem);

                }
                return StatusCode(500, "An error occurred while adding the item to the cart.");

            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while adding the item to the cart.");
            }
        }

        /// <summary>
        /// Updates the quantity of multiple cart items.
        /// </summary>
        /// <param name="updateItemLists">List of items with updated quantities.</param>
        /// <returns>Result of the update operation.</returns>
        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateCartItemQuantity([FromBody] List<UpdateQuantityDTO> updateItemLists)
        {
            try
            {
                if (updateItemLists == null || updateItemLists.Count == 0)
                    return BadRequest("Update list cannot be empty.");

                bool allUpdated = true;
                foreach (var update in updateItemLists)
                {
                    var result = await _cartService.UpdateItemQuantityAsync(update.CartItemId, update.NewQuantity,update.newTotalPrice);
                    if (!result)
                    {
                        allUpdated = false;
                    }
                }

                if (!allUpdated)
                    return StatusCode(500, "Some items could not be updated.");

                return Ok("All quantities updated successfully.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating item quantities.");
            }
        }

        /// <summary>
        /// Removes a specific item from the cart.
        /// </summary>
        /// <param name="itemId">The ID of the cart item to remove.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("remove/{itemId}")]
        public async Task<IActionResult> RemoveItemFromCart(long itemId)
        {
            try
            {
                var result = await _cartService.RemoveItemFromCartAsync(itemId);
                if (!result)
                    return NotFound($"Item with id {itemId} not found in cart.");

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while removing the item from the cart.");
            }
        }

        /// <summary>
        /// Clears all items from the user's cart.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(long userId)
        {
            try
            {
                var result = await _cartService.ClearCartAsync(userId);
                if (!result)
                    return NotFound($"Cart for user id {userId} not found.");

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while clearing the cart.");
            }
        }
        /// <summary>
        /// Submits the user's current cart as an order.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>200 OK if submitted successfully, or an appropriate error.</returns>
        [HttpPost("submit/{userId}")]
        public async Task<IActionResult> SubmitOrder(long userId)
        {
            try
            {
                var result = await _cartService.SubmitOrderAsync(userId);
                if (!result)
                    return BadRequest("Cart is empty or failed to submit order.");

                return Ok("Order submitted successfully.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while submitting the order.");
            }
        }

        /// <summary>
        /// Retrieves the total number of items in the shopping cart for a specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose cart item count is requested.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the count of cart items as an integer.
        /// Returns HTTP 200 with the count on success, or HTTP 500 if an error occurs.
        /// </returns>
        [HttpGet("count")]
        public async Task<IActionResult> getCartItemCount([FromQuery]long userId)
        {
            try
            {
                
                int count=await _cartService.getCartItemsCount(userId);
                return Ok(count);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while counting the count.");
            }
        }

    }
}
