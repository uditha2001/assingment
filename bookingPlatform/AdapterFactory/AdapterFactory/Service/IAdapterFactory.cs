using OrderService.API.DTO;
using ProductService.API.DTO;

namespace AdapterFactory.Service
{
    public interface IAdapterFactory
    {
        public IAdapter GetAdapterById(string adapterId);
        Task<List<ProductDTO>> getAllProducts();
        Task<bool> placeOrder(CheckoutDTO order);
        Task<bool> checkoutOrder(CheckoutDTO order);

    }
    
}
