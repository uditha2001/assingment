using OrderService.API.DTO;
using ProductService.API.DTO;

namespace AdapterFactory.Service
{
    public interface IAdapterFactory
    {
        public IAdapter GetAdapterById(string adapterId);
        Task<List<ProductDTO>> GetAllProductsAsync();
        Task<bool> PlaceOrder(CheckoutDTO order);
        Task<bool> CheckoutOrder(CheckoutDTO order);

    }
    
}
