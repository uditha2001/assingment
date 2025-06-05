using CartService.API.DTO;
using CartService.API.Model.Entities;
using CartService.API.repository.interfaces;
using CartService.API.services.interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartService.Test.Integration
    {
    public class FailingCartRepository : ICartRepository
        {
        public Task<bool> AddOrUpdateCartItemAsync(CartItemEntity item)
            {
            throw new NotImplementedException();
            }

        public Task<bool> ClearCartAsync(long userId)
            {
            throw new NotImplementedException();
            }

        public Task<List<CartItemEntity>> GetCartByUserIdAsync(long userId)
            {
            throw new NotImplementedException();
            }

        public Task<int> GetCartItemsCount(long userId)
            {
            throw new NotImplementedException();
            }

        public Task<bool> RemoveItemFromCartAsync(long cartItemId)
            {
            throw new NotImplementedException();
            }

        public Task<bool> UpdateItemQuantityAsync(long cartItemId, int newQuantity, decimal newTotalPrice)
            {
            throw new NotImplementedException();
            }
        }
    }
