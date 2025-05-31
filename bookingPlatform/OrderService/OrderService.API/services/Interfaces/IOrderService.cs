using OrderService.API.DTO;
using OrderService.API.Models.Entity;

namespace OrderService.API.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for operations related to orders.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Retrieves all orders for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose orders should be retrieved.</param>
        /// <returns>A list of order DTOs.</returns>
        Task<List<OrderDTO>> GetAllOrdersAsync(long userId);

        /// <summary>
        /// Retrieves a specific order by its ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve.</param>
        /// <returns>The order DTO if found; otherwise, null.</returns>
        Task<OrderDTO> GetOrderByIdAsync(int orderId);

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="orderDto">The order data to create.</param>
        /// <returns>The chnaged raws of the newly created order.</returns>
        Task<int> CreateOrderAsync(OrderDTO orderDto);

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="orderDto">The updated order data.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        Task<bool> UpdateOrderAsync(OrderDTO orderDto);
        /// <summary>
        /// Deletes an order by its ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteOrderAsync(int orderId);

        /// <summary>
        /// Maps an <see cref="OrderDTO"/> to an <see cref="OrderEntity"/>, including its items.
        /// </summary>
        /// <param name="dto">The data transfer object representing the order.</param>
        /// <returns>An entity object representing the order and its items.</returns>
        OrderEntity MapToEntity(OrderDTO dto);


        /// <summary>
        /// Maps an <see cref="OrderEntity"/> to an <see cref="OrderDTO"/>, including its items.
        /// </summary>
        /// <param name="entity">The entity object representing the order.</param>
        /// <returns>A DTO object representing the order and its items.</returns>
        OrderDTO MapToDTO(OrderEntity entity);
    }
}
