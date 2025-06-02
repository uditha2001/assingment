using AbcMockService.DTO;
using AdapterFactory.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ProductService.API.DTO;
using System.Text;
using System.Text.Json;

namespace AdapterFactory.Adapters
{
    public class AbcAdapter : IAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceUrls _urls;
        public string SourceName => "abc";



        public AbcAdapter(HttpClient httpClient, IOptions<ServiceUrls> options)
        {
            _httpClient = httpClient;
            _urls = options.Value;
        }

        public bool Checkout()
        {
            return true;
        }

        public async Task<List<ProductDTO>> GetProductContentsFromExternalServiceAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_urls.AbcMockService}/api/v1/products");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<ProductDTO>();
                }

                List<AbcDTO> contentList = await response.Content.ReadFromJsonAsync<List<AbcDTO>>();
                if (contentList != null)
                {
                    var newContentList = contentList.Select(MapAbcToProduct).ToList();
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

        public ProductDTO MapAbcToProduct(AbcDTO abcDto)
        {
            return new ProductDTO
            {
                originId = abcDto.originId,
                Name = abcDto.Name,
                Description = abcDto.Description,
                availableQuantity = abcDto.availableQuantity,
                Price = abcDto.Price,
                owner=abcDto.owner,
                Currency = abcDto.Currency,
                ProductCategoryId = abcDto.ProductCategoryId,
                Attributes = abcDto.Attributes?.Select(a => new ProductAttributesDTO
                {
                    Key = a.Key,
                    provider="Abc",
                    Value = a.Value
                }).ToList() ?? new List<ProductAttributesDTO>(),
                Contents = abcDto.Contents?.Select(c => new ProductContentDTO
                {
                    contentId = c.contentId,
                    provider="Abc",
                    Type = c.Type,
                    Url = c.Url,
                    Description = c.Description
                }).ToList() ?? new List<ProductContentDTO>(),
                provider = "Abc"
            };
        }
        public AbcDTO MapProductToAbc(ProductDTO productDto)
        {
            return new AbcDTO
            {
                originId = productDto.originId,
                Name = productDto.Name,
                Description = productDto.Description,
                availableQuantity= productDto.availableQuantity,
                Price = productDto.Price,
                owner=productDto.owner,
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

        public bool PlaceOrder()
        {
            return true;
        }




    }
}
