using AdapterFactory.Adapters;
using Microsoft.Extensions.Options;
using OrderService.API.DTO;
using ProductService.API.DTO;
using System.Text.Json;


namespace AdapterFactory.Service
{
    public class AdapterFactoryServiceIMPL : IAdapterFactory
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AdapterFactoryServiceIMPL> _logger;
        private readonly ServiceUrls _urls;
        private readonly Dictionary<string, IAdapter> _adapters;

        public AdapterFactoryServiceIMPL(HttpClient httpClient, ILogger<AdapterFactoryServiceIMPL> logger, IOptions<ServiceUrls> options,IEnumerable<IAdapter> adapters)
        {
            _httpClient = httpClient;
            _logger = logger;
            _urls = options.Value;
            _adapters = adapters.ToDictionary(a => a.SourceName.ToLower(), a => a);
        }


        public IAdapter GetAdapterById(string adapterId)
        {
            try
            {
                if (_adapters.TryGetValue(adapterId.ToLower(), out var adapter))
                {
                    return adapter;
                }
                else
                {
                    throw new KeyNotFoundException($"Adapter with id '{adapterId}' not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAdapterById: {ex.Message}");
                throw;
            }
        }

        public async Task<List<ProductDTO>> GetAllProductsAsync()
        {
            var productTasks = _adapters.Select(async kvp =>
            {
                var adapterId = kvp.Key;
                var adapter = kvp.Value;

                try
                {
                    return await adapter.GetProductContentsFromExternalServiceAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching products from adapter '{adapterId}': {ex.Message}");
                    return new List<ProductDTO>();
                }
            });

            var productLists = await Task.WhenAll(productTasks);
            return productLists.SelectMany(p => p).ToList();
        }


        // Return true if no exception occurs, because adapters do not store persistent data,
        // and the order is considered placed successfully.

        public async Task<bool> PlaceOrder(CheckoutDTO order)
        {
            try
            {
                List<ProductDTO> products = new List<ProductDTO>();
                    _logger.LogInformation("Calling ProductService with ID: {ProductId}", order.ProductId);

                    var response = await _httpClient.GetAsync($"{_urls.ProductService}/api/v1/product/byId?productId={order.ProductId}");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"Order submission failed. StatusCode: {response.StatusCode}");
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    ProductDTO product = JsonSerializer.Deserialize<ProductDTO>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in checkoutOrder: {ex.Message}");
                throw;
            }



        }

        public async Task<bool> CheckoutOrder(CheckoutDTO order)

        {
            try
            {
                Console.WriteLine("hello products " + _urls.ProductService);
                var response = await _httpClient.GetAsync($"{_urls.ProductService}/api/v1/product/byId?productId={order.ProductId}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Order submission failed. StatusCode: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                ProductDTO product = JsonSerializer.Deserialize<ProductDTO>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                IAdapter adapter = GetAdapterById(product.provider);
                return adapter.checkout();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error in checkoutOrder: {ex.Message}");
                throw;
            }
           
            
        }
    }
}
