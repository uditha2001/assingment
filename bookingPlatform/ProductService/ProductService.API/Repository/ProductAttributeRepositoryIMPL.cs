using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using ProductService.API.Data;
using ProductService.API.Models.Entities;
using ProductService.API.Repository.RepositoryInterfaces;

namespace ProductService.API.Repository
{
    public class ProductAttributeRepositoryIMPL : IProductAttriuteRepository
    {
        private readonly ProductDbContext _dbContext;

        public ProductAttributeRepositoryIMPL(ProductDbContext dbContext)
        {
            _dbContext=dbContext;
        }
        public async Task<bool> createAttributes(List<ProductAttributesEntity> contents)
        {
            _dbContext.productAttribute.AddRange(contents);
            var result= await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> deleteAttribute(long attributeId)
        {
            var attribute = await _dbContext.productAttribute.FindAsync(attributeId);
            if (attribute == null)
                return false;

            if (!string.IsNullOrEmpty(attribute.provider))
                return false; 

            _dbContext.productAttribute.Remove(attribute);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
        public async Task<bool> UpdateAttributesAsync(List<ProductAttributesEntity> attributeList)
        {
            foreach (var updatedAttribute in attributeList)
            {
                var existingAttribute = await _dbContext.productAttribute.FindAsync(updatedAttribute.AttributeId);
                if (existingAttribute == null)
                    continue;

                if (!string.IsNullOrEmpty(existingAttribute.provider))
                    continue; 

                existingAttribute.Key = updatedAttribute.Key;
                existingAttribute.Value = updatedAttribute.Value;
                existingAttribute.provider = updatedAttribute.provider;
            }
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
        public async Task<List<ProductAttributesEntity>> getAllAttributes(long productId)
        {
            return await _dbContext.productAttribute
                .Where(attr => attr.ProductId == productId)
                .ToListAsync();
        }


    }
}
