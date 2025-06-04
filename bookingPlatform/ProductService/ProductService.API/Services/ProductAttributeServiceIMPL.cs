using ProductService.API.DTO;
using ProductService.API.Models.Entities;
using ProductService.API.Repository.RepositoryInterfaces;
using ProductService.API.Services.serviceInterfaces;

namespace ProductService.API.Services
{
    public class ProductAttributeServiceIMPL : IProductAttributeService
    {
        private readonly IProductAttriuteRepository _repo;
        private readonly ILogger<ProductAttributeServiceIMPL> _logger;
        public ProductAttributeServiceIMPL(IProductAttriuteRepository repo, ILogger<ProductAttributeServiceIMPL> logger)
        {
            _repo = repo;
            _logger = logger;
        }
        public async Task<bool> CreateAttribute(List<ProductAttributesDTO> productAttributesDTOs, long productId)
        {
            try
            {
                if (productAttributesDTOs != null)
                {
                   List<ProductAttributesEntity> productAttributesEntities = new List<ProductAttributesEntity>();
                    foreach (ProductAttributesDTO productAttributesDTO in productAttributesDTOs)
                    {
                        ProductAttributesEntity productAttributesEntity = ToEntity(productAttributesDTO,productId);
                        productAttributesEntities.Add(productAttributesEntity);
                    }
                    bool result = await _repo.CreateAttributes(productAttributesEntities);
                    return result;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.GetType().Name}, Message: {e.Message}, StackTrace: {e.StackTrace}");
                _logger.LogError(e, "An error occurred while processing the request.");
                return false;
            }
        }

        public async Task<bool> DeleteAttribute(long attributeId)
        {
            try
            {
                bool result = await _repo.DeleteAttribute(attributeId);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.GetType().Name}, Message: {e.Message}, StackTrace: {e.StackTrace}");
                _logger.LogError(e, "An error occurred while processing the request.");
                return false;
            }
        }

        public async Task<List<ProductAttributesDTO>> GetAllAttributes(long productId) 
        {
            try
                {
                List<ProductAttributesDTO> productAttributesDTOs = new List<ProductAttributesDTO>();
                List<ProductAttributesEntity> productAttributeEntity = await _repo.GetAllAttributes(productId);
                foreach (ProductAttributesEntity attributesEntity in productAttributeEntity)
                    {
                    ProductAttributesDTO productAttributesDTO = ToDTO(attributesEntity);
                    productAttributesDTOs.Add(productAttributesDTO);
                    }
                return productAttributesDTOs;
                }
            catch (Exception e)
                {
                _logger.LogError(e, "An error occurred while processing the request.");
                throw new Exception($"An error occurred while fetching attributes for product ID {productId}: {e.Message}", e);
                }
        }

        public async Task<bool> UpdateAttribute(List<ProductAttributesDTO> productAttributeDTOs, long productId)
        {
            try
            {
                List<ProductAttributesEntity> productAttributesEntities = new List<ProductAttributesEntity>();
                foreach (ProductAttributesDTO productAttributesDTO in productAttributeDTOs)
                {
                    ProductAttributesEntity productAttributesEntity = ToEntity(productAttributesDTO, productId);
                    productAttributesEntities.Add(productAttributesEntity);
                }
                bool result = await _repo.UpdateAttributesAsync(productAttributesEntities);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.GetType().Name}, Message: {e.Message}, StackTrace: {e.StackTrace}");
                _logger.LogError(e, "An error occurred while processing the request.");
                return false;
            }
        }
        public ProductAttributesEntity ToEntity(ProductAttributesDTO dto, long productId)
        {
            return new ProductAttributesEntity
            {
                AttributeId = dto.attributeId,
                provider = dto.provider,
                Key = dto.Key,
                Value = dto.Value,
                ProductId = productId
            };
        }
        public ProductAttributesDTO ToDTO(ProductAttributesEntity entity)
        {
            return new ProductAttributesDTO
            {
                attributeId = entity.AttributeId,
                provider = entity.provider,
                Key = entity.Key,
                Value = entity.Value
            };
        }
    }
}
