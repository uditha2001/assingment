using ProductService.API.DTO;

namespace AdapterFactory.Service
{
    public interface IAdapter
    {
        string SourceName { get; }
        Task<List<ProductDTO>> GetProductContentsFromExternalServiceAsync();
        bool PlaceOrder();
        bool Checkout();

    }

}
