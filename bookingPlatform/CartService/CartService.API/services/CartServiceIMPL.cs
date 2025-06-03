using CartService.API.DTO;
using CartService.API.Model.Entities;
using CartService.API.repository.interfaces;
using CartService.API.services.interfaces;
using Microsoft.Extensions.Options;
using OrderService.API.DTO;
using System.Text;
using System.Text.Json;

namespace CartService.API.services
{
    public class CartServiceIMPL : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly HttpClient _httpClient;
        private readonly ServiceUrls _urls;



        public CartServiceIMPL(ICartRepository cartRepository, HttpClient httpClient, IOptions<ServiceUrls> options)
        {
            _cartRepository = cartRepository;
            _httpClient = httpClient;
            _urls = options.Value;
        }

        public async Task<bool> AddItemToCartAsync(CartItemDTO item)
        {
            try
            {
                var entity = ToEntity(item, item.userId);
                var updatedEntity = await _cartRepository.AddOrUpdateCartItemAsync(entity);
                return updatedEntity;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to add or update item in cart.", ex);
            }
        }

        public async Task<bool> ClearCartAsync(long userId)
        {
            try
            {
                return await _cartRepository.ClearCartAsync(userId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to clear cart.", ex);
            }
        }

        public async Task<List<CartItemDTO>> GetCartByUserIdAsync(long userId)
        {
            try
            {
                List<CartItemEntity> items = await _cartRepository.GetCartByUserIdAsync(userId);
                List<CartItemDTO> allCarts = new List<CartItemDTO>();
                if (items == null || !items.Any())
                {
                    return allCarts;
                }

                foreach (CartItemEntity cart in items)
                {
                    CartItemDTO cartItemDTO = ToDTO(cart);
                    allCarts.Add(cartItemDTO);

                }
                return allCarts;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve cart items.", ex);
            }
        }

        public async Task<bool> RemoveItemFromCartAsync(long cartItemId)
        {
            try
            {
                return await _cartRepository.RemoveItemFromCartAsync(cartItemId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to remove item from cart.", ex);
            }
        }

        public async Task<bool> UpdateItemQuantityAsync(long cartItemId, int newQuantity, decimal newTotalPrice)
        {
            try
            {

               bool result= await _cartRepository.UpdateItemQuantityAsync(cartItemId, newQuantity, newTotalPrice);
                if (result)
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update item quantity.", ex);
            }
        }
        public async Task<bool> SubmitOrderAsync(long userId)
        {
            try
            {
                List<CartItemEntity> items = await _cartRepository.GetCartByUserIdAsync(userId);

                if (items == null || !items.Any())
                    throw new InvalidOperationException("No items in cart to submit.");

                List<OrderItemsDTO> orders = new List<OrderItemsDTO>();
                decimal totalPrice = 0;

                foreach (CartItemEntity entity in items)
                {
                    orders.Add(new OrderItemsDTO
                    {
                        ProductId = entity.ProductId,
                        quantity = entity.Quantity,
                        itemTotalPrice = entity.itemTotalPrice
                    });

                    totalPrice += entity.itemTotalPrice;
                }

                OrderDTO orderDTO = new OrderDTO
                {
                    items = orders,
                    totalOrderprice = totalPrice,
                    userId = userId
                };

                var json = JsonSerializer.Serialize(orderDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_urls.OrderService}/api/v1/order", content);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Order submission failed. StatusCode: {response.StatusCode}");

               bool result= await _cartRepository.ClearCartAsync(userId);
                if (result)
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to submit order.", ex);
            }
        }



        public CartItemEntity ToEntity(CartItemDTO dto, long userId)
        {
            return new CartItemEntity
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                itemTotalPrice = dto.itemTotalPrice,

            };
        }

        public CartItemDTO ToDTO(CartItemEntity entity)
        {
            return new CartItemDTO
            {
                userId = entity.UserId,
                cartItemId = entity.cartItemId,
                ProductId = entity.ProductId,
                Quantity = entity.Quantity,
                itemTotalPrice = entity.itemTotalPrice,
            };
        }

        public async Task<int> GetCartItemsCount(long userId)
        {
            try
            {
                return await _cartRepository.GetCartItemsCount(userId);
            }
            catch
            (Exception ex)
            {
                throw new ApplicationException(
                    "failed to get count"
                    );
            }
        }
    }
}
