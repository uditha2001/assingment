using ProductService.API.DTO;

namespace AdapterFactory.Service
{
    public interface IAdapter
    {
        Task<List<ProductDTO>> GetProductContentsFromExternalServiceAsync();
        bool placeOrder();
        bool checkout();

    }

}
