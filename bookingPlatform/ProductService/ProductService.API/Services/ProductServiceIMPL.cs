﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OrderService.API.DTO;
using ProductService.API.DTO;
using ProductService.API.Models.Entities;
using ProductService.API.Repository.RepositoryInterfaces;
using ProductService.API.Services.serviceInterfaces;
using System.Text;
using System.Text.Json;

namespace ProductService.API.Services
{
    public class ProductServiceImpl : IProductService
    {
        private readonly IProductRepo _productRepo;
        private readonly ILogger<ProductServiceImpl> _logger;
        private readonly HttpClient _httpClient;
        private readonly ServiceUrls _urls;
        private readonly IProductContentService _productContentService;



        public ProductServiceImpl(
          IProductRepo productRepo,
          ILogger<ProductServiceImpl> logger,
          IOptions<ServiceUrls> options,
          HttpClient httpClient,
          IProductContentService productContentService

         )
        {
            _productRepo = productRepo;
            _httpClient = httpClient;
            _logger = logger;
            _urls = options.Value;
            _productContentService = productContentService;

        }

        public async Task<bool> ImportProducts(ProductDTO productDto)
        {
            try
            {
                ProductEntity product = ProductDTOToEntity(productDto);
                int result=await _productRepo.AddProduct(product);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateProductAsync: {ex.Message}");
                return false;
            }

        }

        public ProductDTO ProductEntityToDTO(ProductEntity entity)
        {
            if (entity == null) return null!;

            return new ProductDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                owner = entity.owner,
                availableQuantity = entity.availableQuantity,
                rate = entity.rate,
                originId = entity.originId,
                provider = entity.Provider,
                Description = entity.Description ?? string.Empty,
                Price = entity.Price,
                Currency = entity.Currency,
                ProductCategoryId = entity.ProductCategoryId,
                createdBy = entity.createdBy,
                Attributes = new List<ProductAttributesDTO>(),
                Contents = new List<ProductContentDTO>()

            };
        }


        public ProductEntity ProductDTOToEntity(ProductDTO dto)
        {
            if (dto == null) return null!;

            return new ProductEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                originId = dto.originId,
                availableQuantity = dto.availableQuantity,
                Description = string.IsNullOrEmpty(dto.Description) ? null : dto.Description,
                Price = dto.Price,
                owner = dto.owner,
                ProductCategoryId = dto.ProductCategoryId,
                createdBy = dto.createdBy,
                Currency = dto.Currency,
                Provider = dto.provider,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
        }
        public async Task<List<ProductDTO>?> UpdateProductsFromAdapterAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_urls.AdapterFactoryService}/api/v1/Adapter");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var productsList = await response.Content.ReadFromJsonAsync<List<ProductDTO>>();

                if (productsList == null || !productsList.Any())
                {
                    return new List<ProductDTO>();
                }

                foreach (var productDto in productsList)
                {
                    var existingProduct = await _productRepo.GetExternalProductsWithOriginIdAsync(productDto);

                    if (existingProduct != null)
                    {
                        var updatedProduct = ProductDTOToEntity(productDto);
                        existingProduct.Name = updatedProduct.Name;
                        existingProduct.Description = updatedProduct.Description;
                        existingProduct.Price = updatedProduct.Price;
                        existingProduct.Currency = updatedProduct.Currency;
                        existingProduct.UpdatedAt = DateTime.UtcNow;
                        existingProduct.originId = updatedProduct.originId;
                        existingProduct.Provider = updatedProduct.Provider;
                        existingProduct.availableQuantity = updatedProduct.availableQuantity;
                        existingProduct.owner = updatedProduct.owner;

                        await _productRepo.RemoveAllProductAttributesByProvider(existingProduct);
                        await _productRepo.RemoveAllProductContentsWhereProviderNotEmpty(existingProduct);

                        ExtractAttributesAndContentFromProductDTO(productDto, existingProduct);
                        await _productRepo.UpdateProductAsync(existingProduct);
                    }
                    else
                    {
                        var newEntity = ProductDTOToEntity(productDto);
                        ExtractAttributesAndContentFromProductDTO(productDto, newEntity);
                        await _productRepo.AddProduct(newEntity);
                    }
                }

                return productsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product and attributes: {ex.Message}");
                return null;
            }
        }


        public async Task<List<ProductDTO>> GetAllProducts()
        {
            try
            {
                var products = await _productRepo.GetAllProducts();
                List<ProductDTO> productsList = new List<ProductDTO>();
                foreach (ProductEntity productEntity in products)
                {
                    ProductDTO productDto = new ProductDTO();
                    productDto = ProductEntityToDTO(productEntity);
                    ExtractAttributesAndContentToDTO(productEntity, productDto);
                    productsList.Add(productDto);
                }
                return productsList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _logger.LogError(e, "An error occurred while processing the request.");
                throw new Exception("failed to get products");
            }

        }

        public async Task<long> CreateProduct(ProductDTO productdto)
        {
            try
            {
                ProductEntity productEntity = ProductDTOToEntity(productdto);
               long id= await _productRepo.SaveProduct(productEntity);
                return id;



            }
            catch (Exception e)
            {
                Console.WriteLine($"Error updating product and attributes: {e.Message}");
                throw new Exception("fiailed create product");
            }
        }

        public void ExtractAttributesAndContentFromProductDTO(ProductDTO productdto, ProductEntity productentity)
        {
            if (productdto.Attributes != null)
            {
                foreach (var attrDto in productdto.Attributes)
                {
                    productentity.Attributes.Add(new ProductAttributesEntity
                    {
                        provider = attrDto.provider,
                        Key = attrDto.Key,
                        Value = attrDto.Value,
                        ProductId = productentity.Id
                    });
                }
            }

            if (productdto.Contents != null)
            {
                foreach (var contentDto in productdto.Contents)
                {
                    productentity.Contents.Add(new ProductContentEntity
                    {
                        provider = contentDto.provider,
                        Type = contentDto.Type,
                        Url = contentDto.Url,
                        Description = contentDto.Description,
                        ProductId = productentity.Id
                    });
                }
            }
        }
        public void ExtractAttributesAndContentToDTO(ProductEntity productEntity, ProductDTO productDTO)
        {
            if (productEntity.Attributes != null)
            {
                productDTO.Attributes = productEntity.Attributes.Select(attr => new ProductAttributesDTO
                {
                    attributeId=attr.AttributeId,
                    provider = attr.provider,
                    Key = attr.Key,
                    Value = attr.Value
                }).ToList();
            }
           
            if (productEntity.Contents != null)
            {
              
                productDTO.Contents = productEntity.Contents.Select(content => new ProductContentDTO
                {
                    contentId=content.ContentId,
                    provider = content.provider,
                    Type = content.Type,
                    Url = content.Url,
                    Description = content.Description
                }).ToList();
            }
        }


        public async Task<bool> DeleteProductAsync(long productId)
        {
            try
            {
                var existingProduct = await _productRepo.GetProductById(productId);

                var contentsToDelete = existingProduct.Contents.ToList();

                foreach (var content in contentsToDelete)
                {
                    var success = await _productContentService.DeleteContent(content.ContentId);

                    if (!success)
                    {
                        _logger.LogWarning("Content ID {ContentId} could not be deleted (may have provider)", content.ContentId);
                    }
                    else
                    {
                        _logger.LogInformation("Content ID {ContentId} successfully deleted", content.ContentId);
                    }
                }



                await _productRepo.DeleteProductAsync(productId);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<List<ProductDTO>> GetInternalSystemProducts()
        {
            try
            {
                List<ProductEntity> productEntityList = await _productRepo.GetInternalSystemProducts();
                List<ProductDTO> products = new List<ProductDTO>();
                foreach (ProductEntity productEntity in productEntityList)
                {
                    ProductDTO productDTO = ProductEntityToDTO(productEntity);
                    products.Add(productDTO);

                }
                return products;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _logger.LogError(e, "An error occurred while processing the request.");
                return new List<ProductDTO>();
            }

        }


        public async Task<bool> SellProducts(List<CheckoutDTO> orderDto)
            {
            bool overallStatus = true;

            try
                {
                foreach (CheckoutDTO orders in orderDto)
                    {
                    ProductEntity product = await _productRepo.GetProductById(orders.ProductId);
                    if (product == null)
                        {
                        _logger.LogWarning($"Product with ID {orders.ProductId} not found.");
                        overallStatus = false;
                        continue;
                        }

                    bool isProductInternal = await _productRepo.CheckInternalSystemProduct(orders.ProductId);

                    if (isProductInternal)
                        {
                        if (product.availableQuantity >= orders.quantity)
                            {
                            await _productRepo.SellProducts(product.Id, product.availableQuantity - orders.quantity);
                            }
                        else
                            {
                            _logger.LogWarning($"Insufficient quantity for product ID {orders.ProductId}.");
                            overallStatus = false;
                            }
                        }
                    else
                        {
                        try
                            {
                            var json = JsonSerializer.Serialize(orders);
                            var order = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await _httpClient.PostAsync($"{_urls.AdapterFactoryService}/api/v1/Adapter", order);

                            if (response.IsSuccessStatusCode)
                                {
                                var responseBody = await response.Content.ReadAsStringAsync();
                                if (!(bool.TryParse(responseBody, out bool isSuccess) && isSuccess))
                                    {
                                    _logger.LogWarning($"Adapter response was invalid or false for product ID {orders.ProductId}.");
                                    overallStatus = false;
                                    }
                                }
                            else
                                {
                                _logger.LogWarning($"Failed to update external product. StatusCode: {response.StatusCode}");
                                overallStatus = false;
                                }
                            }
                        catch (Exception ex)
                            {
                            _logger.LogError(ex, $"HTTP call failed for product ID {orders.ProductId}");
                            overallStatus = false;
                            }
                        }
                    }

                return overallStatus;
                }
            catch (Exception e)
                {
                _logger.LogError(e, "An error occurred while processing SellProducts.");
                throw;
                }
            }


        public async Task<ProductDTO> GetExtranalProductById(long productId)
        {
            try
            {
                ProductEntity product = await _productRepo.GetExternalProductByIdAsync(productId);
                
                    ProductDTO productDto = ProductEntityToDTO(product);
                    return productDto;
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _logger.LogError(e, "An error occurred while processing the request."+e.Message);
                throw;
            }
        }
        public async Task<ProductDTO> GetProductById(long productId)
        {
            try
            {
                ProductEntity product = await _productRepo.GetProductById(productId);
                    ProductDTO productDto = ProductEntityToDTO(product);
                if (product != null)
                    {
                    ExtractAttributesAndContentToDTO(product, productDto);
                    }
                return productDto;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _logger.LogError(e, "An error occurred while processing the request."+e.Message);
                throw;
            }
        }

        public async Task<List<ProductCategoryDTO>> GetAllCategories()
        {
            try
            {
                List<ProductCategoryEntity> categories = await _productRepo.GetAllCategories();
                List<ProductCategoryDTO> result = new List<ProductCategoryDTO>();
                foreach (ProductCategoryEntity entity in categories)
                {
                    ProductCategoryDTO productCategoryDTO = CategoryEntityToDTO(entity);
                    result.Add(productCategoryDTO);
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while processing the request."+e.Message);
                throw new Exception("failed to get categories");
            }
        }
        public ProductCategoryDTO CategoryEntityToDTO(ProductCategoryEntity entity)
        {
            if (entity == null) return null;

            return new ProductCategoryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description
            };
        }

        public ProductCategoryEntity CategoryDToToEntity(ProductCategoryDTO dto)
        {
            if (dto == null) return null;

            return new ProductCategoryEntity
            {
                Id = dto.Id, 
                Name = dto.Name,
                Description = dto.Description
            };
        }

        public async Task<List<ProductDTO>> GetOwnerProducts(long userId)
        {
            try
            {
                List<ProductEntity> allProducts=await _productRepo.GetOwnerProducts(userId);
                List<ProductDTO> result = new List<ProductDTO>();
                foreach (ProductEntity entity in allProducts)
                {
                    ProductDTO productDTO=ProductEntityToDTO(entity);
                    ExtractAttributesAndContentToDTO(entity, productDTO);
                    result.Add(productDTO);
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while processing the request." + e.Message);
                throw new Exception(
                    "failed to get products"
                    );
            }
        }

        public async Task<bool> GetCheckout(CheckoutDTO order)
        {
            try
            {
                bool isProductInternal = await _productRepo.CheckInternalSystemProduct(order.ProductId);
                if (isProductInternal)
                {
                    ProductEntity productEntity = await _productRepo.Chekout(order);
                    if (productEntity.availableQuantity >= order.quantity)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    var json = JsonSerializer.Serialize(order);
                    var orderDetails = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync($"{_urls.AdapterFactoryService}/api/v1/adapter/checkout", orderDetails);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to update product: {response.StatusCode}");
                        return false;
                    }

                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while processing the request." + e.Message);
                throw new Exception(
                    "error occur");
            }


        }

        public async Task<bool> UpdateProduct(ProductDTO productDto)
        {
            try
            {
                var existingProduct = await _productRepo.GetProductById(productDto.Id);

                if (existingProduct != null)
                {
                    var updatedProduct = ProductDTOToEntity(productDto);
                    existingProduct.Name = updatedProduct.Name;
                    existingProduct.Description = updatedProduct.Description;
                    existingProduct.Price = updatedProduct.Price;
                    existingProduct.Currency = updatedProduct.Currency;
                    existingProduct.UpdatedAt = DateTime.UtcNow;
                    existingProduct.originId = updatedProduct.originId;
                    existingProduct.Provider = updatedProduct.Provider;
                    existingProduct.availableQuantity = updatedProduct.availableQuantity;
                    existingProduct.owner = updatedProduct.owner;

                    await _productRepo.RemoveAllProductAttributesByProvider(existingProduct);
                    await _productRepo.RemoveAllProductContentsWhereProviderNotEmpty(existingProduct);
                    

                    if (productDto.Attributes != null)
                    {
                        foreach (var attrDto in productDto.Attributes)
                        {
                            existingProduct.Attributes.Add(new ProductAttributesEntity
                            {
                                provider = attrDto.provider,
                                Key = attrDto.Key,
                                Value = attrDto.Value,
                                ProductId = existingProduct.Id
                            });
                        }
                    }

                    if (productDto.Contents != null)
                    {
                        foreach (var contentDto in productDto.Contents)
                        {

                            existingProduct.Contents.Add(new ProductContentEntity
                            {
                                provider = contentDto.provider,
                                Type = contentDto.Type,
                                Url = contentDto.Url,
                                Description = contentDto.Description,
                                ProductId = existingProduct.Id
                            });
                        }
                    }
                   int effectRaws= await _productRepo.UpdateProductAsync(existingProduct);
                    if (effectRaws > 0)
                        {
                        return true;
                        }
                    else
                        {
                        return false;
                        }

                }
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while processing the request." + e.Message);
                throw new Exception(
                    "error occur");
            }
            

        
    }
    }
}
