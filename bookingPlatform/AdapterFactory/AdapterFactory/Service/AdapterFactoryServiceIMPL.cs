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

        public AdapterFactoryServiceIMPL(HttpClient httpClient, ILogger<AdapterFactoryServiceIMPL> logger, IOptions<ServiceUrls> options, IEnumerable<IAdapter> adapters)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _urls = options?.Value ?? throw new ArgumentNullException(nameof(options));
            if (adapters == null) throw new ArgumentNullException(nameof(adapters));
            _adapters = adapters.ToDictionary(a => a.SourceName.ToLower(), a => a);
        }

        public IAdapter GetAdapterById(string adapterId)
        {
            if (string.IsNullOrWhiteSpace(adapterId))
                throw new ArgumentException("Adapter ID must not be null or empty.", nameof(adapterId));

            if (_adapters.TryGetValue(adapterId.ToLower(), out var adapter))
            {
                return adapter;
            }

            _logger.LogError("Adapter with id '{AdapterId}' not found.", adapterId);
            throw new KeyNotFoundException($"Adapter with id '{adapterId}' not found.");
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
                    _logger.LogError(ex, "Error fetching products from adapter '{AdapterId}'", adapterId);
                    return new List<ProductDTO>();
                }
            });

            var productLists = await Task.WhenAll(productTasks);
            return productLists.SelectMany(p => p).ToList();
        }

        public async Task<bool> PlaceOrder(CheckoutDTO order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            try
            {
                _logger.LogInformation("Calling ProductService with ID: {ProductId}", order.ProductId);

                var response = await _httpClient.GetAsync($"{_urls.ProductService}/api/v1/product/byId?productId={order.ProductId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Order submission failed. StatusCode: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException($"Order submission failed. StatusCode: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductDTO>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (product == null)
                {
                    _logger.LogError("Product deserialization failed for ProductId: {ProductId}", order.ProductId);
                    throw new InvalidOperationException("Product deserialization failed.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PlaceOrder for ProductId: {ProductId}", order?.ProductId);
                throw;
            }
        }

        public async Task<bool> CheckoutOrder(CheckoutDTO order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            try
            {
                _logger.LogInformation("Calling ProductService for checkout with ID: {ProductId}", order.ProductId);

                var response = await _httpClient.GetAsync($"{_urls.ProductService}/api/v1/product/byId?productId={order.ProductId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Order submission failed. StatusCode: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException($"Order submission failed. StatusCode: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductDTO>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (product == null || string.IsNullOrWhiteSpace(product.provider))
                {
                    _logger.LogError("Product deserialization failed or provider missing for ProductId: {ProductId}", order.ProductId);
                    throw new InvalidOperationException("Product deserialization failed or provider missing.");
                }

                var adapter = GetAdapterById(product.provider);
                return adapter.Checkout();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckoutOrder for ProductId: {ProductId}", order?.ProductId);
                throw;
            }
        }
    }
}
