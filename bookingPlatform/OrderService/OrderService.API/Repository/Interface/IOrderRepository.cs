using OrderService.API.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderService.API.Repository.Interface
{
    /// <summary>
    /// Defines the contract for data access operations related to orders.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Retrieves all orders for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose orders are requested.</param>
        /// <returns>A list of order entities.</returns>
        Task<List<OrderEntity>> GetOrdersByUserIdAsync(long userId);

        /// <summary>
        /// Retrieves a single order by its ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve.</param>
        /// <returns>The order entity if found; otherwise, null.</returns>
        Task<OrderEntity> GetOrderByIdAsync(long orderId);

        /// <summary>
        /// Adds a new order to the database.
        /// </summary>
        /// <param name="order">The order entity to add.</param>
        /// <returns>The ID of the newly created order.</returns>
        Task<int> CreateOrderAsync(OrderEntity order);

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="order">The updated order entity.</param>
        /// <returns>True if update is successful; otherwise, false.</returns>
        Task<bool> UpdateOrderAsync(OrderEntity order);

        /// <summary>
        /// Deletes an order by its ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to delete.</param>
        /// <returns>True if the order was successfully deleted; otherwise, false.</returns>
        Task<bool> DeleteOrderAsync(long orderId);
    }
}
