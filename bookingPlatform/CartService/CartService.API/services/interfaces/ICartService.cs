using CartService.API.DTO;
using CartService.API.Model.Entities;

namespace CartService.API.services.interfaces
{
    public interface ICartService
    {
        /// <summary>
        /// Gets the cart for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The user's cart and its items.</returns>
        Task<List<CartItemDTO>> GetCartByUserIdAsync(long userId);

        /// <summary>
        /// Adds an item to the user's cart. If the item already exists, its quantity will be increased.
        /// </summary>
        /// <param name="item">The cart item to be added.</param>
        /// <returns>the bool value.</returns>
        Task<bool> AddItemToCartAsync(CartItemDTO item);

        /// <summary>
        /// Updates the quantity of a specific item in the cart.
        /// </summary>
        /// <param name="cartItemId">The ID of the cart item.</param>
        /// <param name="newQuantity">The new quantity to be set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<bool> UpdateItemQuantityAsync(long cartItemId, int newQuantity,decimal newTotalPrice);

        /// <summary>
        /// Removes a specific item from the cart.
        /// </summary>
        /// <param name="cartItemId">The ID of the cart item to remove.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<bool> RemoveItemFromCartAsync(long cartItemId);

        /// <summary>
        /// Clears all items from the user's cart.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<bool> ClearCartAsync(long userId);

        /// <summary>
        /// Maps a CartItemDTO to a CartItemEntity.
        /// </summary>
        /// <param name="dto">The cart item data transfer object.</param>
        /// <param name="userId">The user ID to associate with the entity.</param>
        /// <returns>A CartItemEntity with properties copied from the DTO and UserId set.</returns>
        CartItemEntity ToEntity(CartItemDTO dto, long userId);

        /// <summary>
        /// Maps a CartItemEntity to a CartItemDTO.
        /// </summary>
        /// <param name="entity">The cart item entity.</param>
        /// <returns>A CartItemDTO with properties copied from the entity.</returns>
        CartItemDTO ToDTO(CartItemEntity entity);

        /// <summary>
        /// Submits an order for the user's current cart.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation. Returns true if the order was submitted successfully.</returns>
        Task<bool> SubmitOrderAsync(long userId);

        /// <summary>
        /// Retrieves the total number of items in the cart for a given user.
        /// </summary>
        /// <param name="userId">The ID of the user whose cart items count is requested.</param>
        /// <returns>
        /// A <see cref="Task{Int32}"/> representing the asynchronous operation, 
        /// containing the total count of cart items for the specified user.
        /// </returns>
        /// <exception cref="ApplicationException">Thrown when the count retrieval fails.</exception>
        Task<int> GetCartItemsCount(long userId);

    }

}
