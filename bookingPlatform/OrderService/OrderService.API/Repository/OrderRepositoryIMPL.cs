using Microsoft.EntityFrameworkCore;
using OrderService.API.Data;
using OrderService.API.Models.Entity;
using OrderService.API.Repository.Interface;

namespace OrderService.API.Repository
{
    public class OrderRepositoryIMPL : IOrderRepository
    {
        private readonly OrderDbContext _dbContext;
        public OrderRepositoryIMPL(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateOrderAsync(OrderEntity order)
        {
            _dbContext.Order.Add(order);
            int changeraws=await _dbContext.SaveChangesAsync();
            return changeraws;
        }

        public async Task<bool> DeleteOrderAsync(long orderId)
        {
            var order = await _dbContext.Order.FirstOrDefaultAsync(O => O.orderId == orderId);
            if (order != null)
            {
                _dbContext.Order.Remove(order);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new KeyNotFoundException($"Product with ID {orderId} was not found.");

            }

        }

        public async Task<OrderEntity> GetOrderByIdAsync(long orderId)
        {
          OrderEntity order= await _dbContext.Order.Include(O=>O.items).FirstOrDefaultAsync(O => O.orderId == orderId);
            return order;
        }

        public async Task<List<OrderEntity>> GetOrdersByUserIdAsync(long userId)
        {
            List<OrderEntity> orders=await _dbContext.Order.Include(O=>O.items).Where(O=>O.userId==userId).ToListAsync();
            return orders;
        }

        public async Task<bool> UpdateOrderAsync(OrderEntity order)
        {
            var existingOrdr = await _dbContext.Order.Include(O => O.items)
                .FirstOrDefaultAsync(o=>o.orderId==order.orderId);
            if (existingOrdr == null)
                return false;
            existingOrdr.totalOrderprice = order.totalOrderprice;

            _dbContext.orderItems.RemoveRange(existingOrdr.items);
            existingOrdr.items = order.items;
            await _dbContext.SaveChangesAsync();
            return true;

        }
    }
    }
