using Microsoft.EntityFrameworkCore;
using OrderService.API.DTO;
using ProductService.API.Data;
using ProductService.API.DTO;
using ProductService.API.Models.Entities;
using ProductService.API.Repository.RepositoryInterfaces;

namespace ProductService.API.Repository
{
    public class ProductRepositoryIMPL : IProductRepo
    {
        private readonly ProductDbContext _dbContext;
        public ProductRepositoryIMPL(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddProductAsync(ProductEntity product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
        }


        public async Task RemoveAllProductAttributesByProvider(ProductEntity existingProduct)
        {
            var filteredAttributes = existingProduct.Attributes
                .ToList();

            _dbContext.productAttribute.RemoveRange(filteredAttributes);
            await _dbContext.SaveChangesAsync();
        }


        public async Task RemoveAllProductContentsWhereProviderNotEmpty(ProductEntity existingProduct)
        {
            var filteredContents = existingProduct.Contents
                .ToList();

            _dbContext.Content.RemoveRange(filteredContents);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<ProductEntity?> GetProductIfExistsAsync(ProductDTO productDto)
        {
            return await _dbContext.Products
                .Include(p => p.Attributes)
                .Include(p => p.Contents)
                .FirstOrDefaultAsync(p =>
                    p.originId == productDto.originId &&
                    p.Provider == productDto.provider);
        }


        async Task IProductRepo.addProduct(ProductEntity productEntity)
        {
             _dbContext.Products.Add(productEntity);
            await _dbContext.SaveChangesAsync();

        }
        public async Task UpdateProductAsync(ProductEntity product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<ProductEntity>> getAllProducts()
        {
            return await _dbContext.Products.Include(p => p.Attributes)
                .Include(p => p.Contents).ToListAsync();
        }

        public async Task<long> saveProduct(ProductEntity productEntity)
        {
            _dbContext.Products.Add(productEntity);
            await _dbContext.SaveChangesAsync();
            return productEntity.Id;
        }

        public async  Task deleteProductAsync(long productId)
        {
            var product = await _dbContext.Products
           .FirstOrDefaultAsync(p => p.Id == productId && p.Provider=="" && p.originId==-1);
            if (product != null)
            {
                _dbContext.Products .Remove(product);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Product with ID {productId} was not found.");

            }
        }

        public async Task<List<ProductEntity>> getInternalSystemProducts()
        {
            List<ProductEntity> products =await _dbContext.Products.Where(p => p.Provider == "" && p.originId == -1)
            .ToListAsync();
            return products;
        }

        public async Task<bool> sellProducts(long productId,int restItemsCount)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product != null)
            {
                product.availableQuantity=restItemsCount;
            }
           int effectRaw= await _dbContext.SaveChangesAsync();
            return effectRaw > 0;
            
        }


        public async Task<ProductEntity> getExternalProductByIdAsync(long productId)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId && !string.IsNullOrEmpty(p.Provider));
        }

        public async Task<List<ProductCategoryEntity>> getAllCategories()
        {
            return await _dbContext.productCategory.ToListAsync();
        }


        public async Task<List<ProductEntity>> getOwnerProducts(long userId)
        {
            var userIdStr = userId.ToString();

            List<ProductEntity> products = await _dbContext.Products
                .Include(p => p.Attributes)
                .Include(p => p.Contents)
                .Where(p => p.createdBy == userId)
                .ToListAsync();
        return products;
        }

        public async Task<ProductEntity> GetProductById(long productId)
        {
            return await _dbContext.Products.Include(p => p.Contents).Include(p => p.Attributes)
                         .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<ProductEntity> chekout(CheckoutDTO order)
        {
            ProductEntity product = await _dbContext.Products.FirstOrDefaultAsync(p=>p.Id==order.ProductId);
            if (product != null)
            {
                return product;
            }
            return new ProductEntity();

        }

        public async Task<bool> checkInternalSystemProduct(long productId)
        {
            ProductEntity product = await _dbContext.Products.FirstOrDefaultAsync(p=>p.Id==productId && p.Provider=="");
            if (product == null)
            {
                return false;
            }
            return true;
        }
    }
}
