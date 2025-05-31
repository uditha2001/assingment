using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.API.DTO;
using OrderService.API.Models.Entity;
using OrderService.API.Repository.Interface;
using OrderService.API.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace OrderService.API.services
{
    public class OrderServiceIMPL : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly ILogger<OrderServiceIMPL> _logger;
        private readonly HttpClient _httpClient;



        public OrderServiceIMPL(IOrderRepository repo, ILogger<OrderServiceIMPL> logger, HttpClient httpClient)
        {
            _repo = repo;
            _logger = logger;
            _httpClient = httpClient;
        }
        public async Task<int> CreateOrderAsync(OrderDTO orderDto)
        {

            try
            {
                OrderEntity orderentity = MapToEntity(orderDto);
                await  _repo.CreateOrderAsync(orderentity);
                return 1;
            }
            catch(Exception e) {
                Console.WriteLine($"Error in CreateProductAsync: {e.Message}");
                _logger.LogError(e, "An error occurred while processing the request.");
                throw;
            }
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
               bool result= await _repo.DeleteOrderAsync(orderId);
               return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in dleteproductasync: {e.Message}");
                _logger.LogError(e, "An error occurred while processing the request.");
                throw;
            }
        }

        public async Task<List<OrderDTO>> GetAllOrdersAsync(long userId)
        {
            try
            {
                List<OrderDTO> orderDTOList = new List<OrderDTO>();
                List<OrderEntity> orderEntityList=await _repo.GetOrdersByUserIdAsync(userId);
                foreach (OrderEntity orderentity in orderEntityList)
                {
                    OrderDTO orderDTO = MapToDTO(orderentity);
                    orderDTOList.Add(orderDTO);
                }
                return orderDTOList;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in dleteproductasync: {e.Message}");
                _logger.LogError(e, "An error occurred while processing the request.");
                throw;

            }
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int orderId)
        {
            try
            {
                OrderEntity orderEntityList = await _repo.GetOrderByIdAsync(orderId);
                OrderDTO orderDTO=MapToDTO(orderEntityList);
                return orderDTO;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in dleteproductasync: {e.Message}");
                _logger.LogError(e, "An error occurred while processing the request.");
                throw;
            }
        }

        public async Task<bool> UpdateOrderAsync(OrderDTO orderDto)
        {
            try
            {
                OrderEntity orderEntity=MapToEntity(orderDto);
                bool result = await _repo.UpdateOrderAsync(orderEntity);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in dleteproductasync: {e.Message}");
                _logger.LogError(e, "An error occurred while processing the request.");
                throw;
            }
        }
        public  OrderEntity MapToEntity(OrderDTO dto)
        {
            return new OrderEntity
            {
                orderId = dto.orderId,
                userId = dto.userId,
                totalOrderprice = dto.totalOrderprice,
                items = dto.items.Select(itemDto => new OrderItemEntity
                {
                   orderItemId = itemDto.orderItemId,
                    orderId = itemDto.orderId,
                    quantity = itemDto.quantity,
                    ProductId = itemDto.ProductId,
                    itemTotalPrice = itemDto.itemTotalPrice
                }).ToList()
            };
        }
        public  OrderDTO MapToDTO(OrderEntity entity)
        {
            return new OrderDTO
            {
                orderId = (int)entity.orderId,
                userId = entity.userId,
                totalOrderprice = entity.totalOrderprice,
                items = entity.items.Select(item => new OrderItemsDTO
                {
                    orderItemId = item.orderItemId,
                    orderId = item.orderId,
                    quantity = item.quantity,
                    ProductId = item.ProductId,
                    itemTotalPrice = item.itemTotalPrice
                }).ToList()
            };
        }
       



    }
}
