using CartService.API.Data;
using CartService.API.Model.Entities;
using CartService.API.repository.interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartService.API.repository
{
    public class CartRepositoryIMPL : ICartRepository
    {
        private readonly CartDbContext _dbContext;
        public CartRepositoryIMPL(CartDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> AddOrUpdateCartItemAsync(CartItemEntity item)
        {
            CartItemEntity entity = await _dbContext.cartItems.FirstOrDefaultAsync(i => i.UserId == item.UserId && i.ProductId == item.ProductId);
            if (entity == null)
            {
               await _dbContext.cartItems.AddAsync(item);
            }
            else
            {
                entity.Quantity=item.Quantity;
                entity.itemTotalPrice=item.itemTotalPrice;
                _dbContext.cartItems.Update(entity);
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(long userId)
        {
           List<CartItemEntity> cartItem =await _dbContext.cartItems.Where(i => i.UserId == userId).ToListAsync();
            if (!cartItem.Any())
            {
                return false;
            }
            _dbContext.cartItems.RemoveRange(cartItem);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<CartItemEntity>> GetCartByUserIdAsync(long userId)
        {
            List<CartItemEntity> cartItems =await  _dbContext.cartItems.Where(i => i.UserId==userId).ToListAsync();
            return cartItems;
        }

        public async Task<int> GetCartItemsCount(long userId)
        {
            return await _dbContext.cartItems
                .Where(ci => ci.UserId == userId)
                .CountAsync();
        }



        public async Task<bool> RemoveItemFromCartAsync(long cartItemId)
        {
            CartItemEntity entity = await _dbContext.cartItems.FirstOrDefaultAsync(i => i.cartItemId == cartItemId);
            if (entity==null)
            {
                return false;
            }
            _dbContext.cartItems.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateItemQuantityAsync(long cartItemId, int newQuantity, decimal newTotalPrice)
        {
            CartItemEntity entity = await _dbContext.cartItems.FirstOrDefaultAsync(i => i.cartItemId == cartItemId);

            if (entity == null)
            {
                return false; 
            }

            entity.Quantity = newQuantity;
            entity.itemTotalPrice = newTotalPrice;

            await _dbContext.SaveChangesAsync();

            return true;
        }

    }
}
