using ProductService.API.Data;
using ProductService.API.Models.Entities;
using ProductService.API.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace ProductService.API.Repository
{
    public class ProductContentRepositoryIMPL : IProductContentRepository
    {
        private readonly ProductDbContext _dbContext;
        public ProductContentRepositoryIMPL(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool>  CreateContentAsync(ProductContentEntity productContentEntity)
        {
            _dbContext.Content.Add(productContentEntity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteContentByIdAsync(long contentId)
        {
            var content = await _dbContext.Content.FirstOrDefaultAsync(c=>c.ContentId==contentId);
            if (content == null)
                return false;

            if (content.provider == "")
            {
                _dbContext.Content.Remove(content);
                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }

            return false;
        }



        public async Task<List<ProductContentEntity>> GetAllContentAsync(long productId)
        {
            return await _dbContext.Content.Where(c=>c.ProductId==productId).ToListAsync();
        }

        public async Task<ProductContentEntity> GetContentByIdAsync(long id)
        {
            return await _dbContext.Content.FirstOrDefaultAsync(pc => pc.ContentId == id);
        }

        public async Task<bool> UpdateContentAsync(ProductContentEntity updatedContent)
        {
            var existingContent = await _dbContext.Content
                .FindAsync(updatedContent.ContentId);

            if (existingContent == null)
                return false;

            if (string.IsNullOrEmpty(existingContent.provider))
            {
                existingContent.Type = updatedContent.Type;
                existingContent.Url = updatedContent.Url;
                existingContent.Description = updatedContent.Description;

                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }

            return false;
        }


    }
}
