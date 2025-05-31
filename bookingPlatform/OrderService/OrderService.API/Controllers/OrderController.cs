using Microsoft.AspNetCore.Mvc;
using OrderService.API.DTO;
using OrderService.API.Services.Interfaces;

namespace OrderService.API.Controllers
{
    [Route("api/v1/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService) {
            _orderService = orderService;
        }
        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <returns>List of orders.</returns>
        [HttpGet]
        public async Task<ActionResult<OrderDTO>> GetAllOrders([FromQuery] long userId)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving orders: {ex.Message}");
            }
            ;
        }

        /// <summary>
        /// Retrieves a specific order by ID.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The order details.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {

            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null || order.orderId == 0)
                    return NotFound($"Order with ID {id} not found");

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving order: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="order">The order object to create.</param>
        /// <returns>Result of the create operation.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO order)
        {

            try
            {
                int result = await _orderService.CreateOrderAsync(order);
                if (result > 0)
                    return CreatedAtAction(nameof(GetOrderById), new { id = order.orderId }, order);
                else
                    return BadRequest("Failed to create order");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating order: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="id">The ID of the order to update.</param>
        /// <param name="order">The updated order object.</param>
        /// <returns>Result of the update operation.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderDTO order)
        {
            try
            {
                bool updated = await _orderService.UpdateOrderAsync(order);
                if (!updated)
                    return NotFound($"Order with ID {order.orderId} not found");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating order: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes an order by ID.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns>Result of the delete operation.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                bool deleted = await _orderService.DeleteOrderAsync(id);
                if (!deleted)
                    return NotFound($"Order with ID {id} not found");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting order: {ex.Message}");
            }
        }
    }
}
