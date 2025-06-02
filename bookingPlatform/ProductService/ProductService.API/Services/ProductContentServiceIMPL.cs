using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.API.Data;
using ProductService.API.DTO;
using ProductService.API.Models.Entities;
using ProductService.API.Repository.RepositoryInterfaces;
using ProductService.API.Services.serviceInterfaces;
using Quartz;
using System.Net.Http;

namespace ProductService.API.Services
{
    public class ProductContentServiceIMPL : IProductContentService
    {
        private readonly IProductContentRepository _repository;
        private readonly ILogger<ProductContentServiceIMPL> _logger;
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _env;


        public ProductContentServiceIMPL(IProductContentRepository repository, ILogger<ProductContentServiceIMPL> logger, IWebHostEnvironment env)
        {
            _repository = repository;
            _logger = logger;
            _httpClient = new HttpClient();
            _env = env;

        }

        public async Task<bool> createContent(ProductContentDTO productContentDTO, long productId)
        {
            try
            {

                var entity = ToEntity(productContentDTO, productId);
                return await _repository.CreateContentAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product content for ProductId: {ProductId}", productId);
                return false;
            }
        }

        public async Task<bool> deleteContent(long id)
        {
            try
            {
                var content = await _repository.GetContentByIdAsync(id);
                if (content == null)
                {
                    _logger.LogWarning("Content with ID {ContentId} not found", id);
                    return false;
                }

                var wwwrootPath = _env.WebRootPath;
                var filePath = Path.Combine(wwwrootPath, content.Url.Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                else
                {
                    _logger.LogWarning("File not found at path {FilePath}", filePath);
                }

                return await _repository.DeleteContentByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product content with ContentId: {ContentId}", id);
                return false;
            }
        }


        public async Task<List<ProductContentDTO>> getAllContent(long productId)
        {
            try
            {
                var entities = await _repository.GetAllContentAsync(productId);
                return entities.Select(ToDTO).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all product content.");
                return new List<ProductContentDTO>();
            }
        }

      


        public ProductContentDTO ToDTO(ProductContentEntity entity)
        {
            return new ProductContentDTO
            {
                provider = entity.provider,
                contentId = entity.ContentId,
                Type = entity.Type,
                Url = entity.Url,
                Description = entity.Description
            };
        }

        public ProductContentEntity ToEntity(ProductContentDTO dto, long productId)
        {
            return new ProductContentEntity
            {
                ContentId = dto.contentId,
                provider = dto.provider,
                Type = dto.Type,
                Url = dto.Url,
                Description = dto.Description,
                ProductId = productId
            };
        }
        public async Task<bool> SaveProductImagesAsync(long productId, List<IFormFile> images)
        {
            if (images == null || images.Count == 0)
                return false;
            var wwwrootPath = _env.WebRootPath;
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }
            var folderPath = Path.Combine(wwwrootPath, "uploads", "products", productId.ToString());

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var productContents = new List<ProductContentDTO>();
            long contentIdCounter = 1;

            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var relativeUrl = Path.Combine("uploads", "products", productId.ToString(), fileName).Replace("\\", "/");
                  

                    ProductContentDTO contentDto = new ProductContentDTO
                    {
                        provider = "",
                        Type = image.ContentType,
                        Url = relativeUrl,
                        Description = $"Image for product {productId}"
                    };
                    await createContent(contentDto,productId);

                }
            }

            return true;
        }

    }
}
