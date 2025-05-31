using AdapterFactory.Service;
using CdeMockService.DTO;
using ProductService.API.DTO;


namespace AdapterFactory.Adapters
{
    public class CdeAdapter : IAdapter
    {
        private readonly HttpClient _httpClient;

        public CdeAdapter(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public ProductDTO MapCdeToProduct(CdeDTO cdeDTO)
        {
            return new ProductDTO
            {
                originId = cdeDTO.originId,
                availableQuantity = cdeDTO.availableQuantity,
                Name = cdeDTO.Name,
                owner = cdeDTO.owner,
                Description = cdeDTO.Description,
                Price = cdeDTO.Price,
                Currency = cdeDTO.Currency,
                ProductCategoryId = cdeDTO.ProductCategoryId,
                Attributes = cdeDTO.Attributes?.Select(a => new ProductAttributesDTO
                {
                    Key = a.Key,
                    Value = a.Value,
                    provider="Cde"
                }).ToList() ?? new List<ProductAttributesDTO>(),
                Contents = cdeDTO.Contents?.Select(c => new ProductContentDTO
                {
                    contentId = c.contentId,
                    Type = c.Type,
                    Url = c.Url,
                    Description = c.Description,
                    provider = "Cde"
                }).ToList() ?? new List<ProductContentDTO>(),
                provider="Cde"
            };
        }
        public CdeDTO MapProductToCde(ProductDTO productDto)
        {
            return new CdeDTO
            {
                originId = productDto.originId,
                Name = productDto.Name,
                owner=productDto.owner,
                availableQuantity=productDto.availableQuantity,
                Description = productDto.Description,
                Price = productDto.Price,
                Currency = productDto.Currency,
                ProductCategoryId = productDto.ProductCategoryId,
                Attributes = productDto.Attributes?.Select(a => new ProductAttributesDTO
                {
                    Key = a.Key,
                    Value = a.Value
                }).ToList() ?? new List<ProductAttributesDTO>(),
                Contents = productDto.Contents?.Select(c => new ProductContentDTO
                {
                    contentId = c.contentId,
                    Type = c.Type,
                    Url = c.Url,
                    Description = c.Description
                }).ToList() ?? new List<ProductContentDTO>()
            };
        }
        public async Task<List<ProductDTO>> GetProductContentsFromExternalServiceAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://cde-mock-service:8080/api/v1/product");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<ProductDTO>();
                }

                List<CdeDTO> contentList = await response.Content.ReadFromJsonAsync<List<CdeDTO>>();
                if (contentList != null)
                {
                    var newContentList = contentList.Select(MapCdeToProduct).ToList();
                    return newContentList;
                }

                return new List<ProductDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while calling external service: {ex.Message}");
                return new List<ProductDTO>();
            }
        }
        public bool placeOrder()
        {
            return true;
        }

        public bool checkout()
        {
            return true;
        }
    }
}
