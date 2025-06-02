using OrderService.API.DTO;
using ProductService.API.Data;
using ProductService.API.DTO;
using ProductService.API.Models.Entities;

namespace ProductService.API.Repository.RepositoryInterfaces
{
    public interface IProductRepo
    {
        /// <summary>
        /// Removes all product attributes from the specified product entity that have a non-empty provider value.
        /// </summary>
        /// <param name="existingProduct">The <see cref="ProductEntity"/> containing the attributes to be removed.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RemoveAllProductAttributesByProvider(ProductEntity existingProduct);

        /// <summary>
        /// Removes all product content entries from the specified product entity that have a non-empty provider value.
        /// </summary>
        /// <param name="existingProduct">The <see cref="ProductEntity"/> containing the content entries to be removed.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RemoveAllProductContentsWhereProviderNotEmpty(ProductEntity existingProduct);

        /// <summary>
        /// Adds a new product to the database asynchronously.
        /// </summary>
        /// <param name="product">The <see cref="ProductEntity"/> object to be added.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddProduct(ProductEntity productEntity);

        /// <summary>
        /// Updates an existing product entity in the database.only allow update entity created via internal system
        /// </summary>
        /// <param name="product">The <see cref="ProductEntity"/> to be updated.</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task UpdateProductAsync(ProductEntity product);

        /// <summary>
        /// Retrieves all product entities from the database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a list of <see cref="ProductEntity"/> objects.
        /// </returns>
        Task<List<ProductEntity>> GetAllProducts();

        /// <summary>
        /// Saves a new product entity to the database.
        /// </summary>
        /// <param name="productEntity">The <see cref="ProductEntity"/> to be saved.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task<long> SaveProduct(ProductEntity productEntity);

        /// <summary>
        /// Deletes a product with the specified ID if it was created internally 
        /// (i.e., has an empty Provider and originId of -1).
        /// </summary>
        /// <param name="productId">The ID of the product to be deleted.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when a product with the specified ID does not exist or does not match the internal system criteria.
        /// </exception>
        Task DeleteProductAsync(long productId);

        /// <summary>
        /// Retrieves all products created by the internal system, identified by an empty Provider and originId of -1.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a list of <see cref="ProductEntity"/> objects.
        /// </returns>
        Task<List<ProductEntity>> GetInternalSystemProducts();

        /// <summary>
        /// Updates the available quantity of a product after a sale.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="restItemsCount">The new available quantity after the sale.</param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> indicating whether the update was successful (true) or not (false).
        /// </returns>
        /// <remarks>
        /// Fetches the product by ID from the database, updates its available quantity,
        /// then saves the changes asynchronously.
        /// </remarks>
        Task<bool> SellProducts(long productId, int restItemsCount);

        /// <summary>
        /// Retrieves all product categories from the database.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{List{ProductCategoryEntity}}"/> containing a list of all product category entities.
        /// </returns>
        /// <remarks>
        /// This method fetches all records from the productCategory table asynchronously.
        /// </remarks>
        Task<List<ProductCategoryEntity>> GetAllCategories();

        /// <summary>
        /// Retrieves an external product by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product to retrieve.</param>
        /// <returns>
        /// A <see cref="Task{ProductEntity}"/> representing the asynchronous operation,
        /// containing the product entity if found and the product has a non-empty Provider; otherwise, null.
        /// </returns>
        /// <remarks>
        /// This method queries the database for a product with the specified ID where the Provider field is not null or empty,
        /// indicating that the product is from an external source.
        /// </remarks>
        Task<ProductEntity> GetExternalProductByIdAsync(long productId);

        /// <summary>
        /// Retrieves all products created by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose products are to be retrieved.</param>
        /// <returns>
        /// A <see cref="Task{List{ProductEntity}}"/> containing a list of products owned by the specified user,
        /// including their attributes and contents.
        /// </returns>
        /// <remarks>
        /// This method queries the database asynchronously, including related Attributes and Contents entities,
        /// filtering products by the creator's user ID.
        /// </remarks>
        Task<List<ProductEntity>> GetOwnerProducts(long userId);

        /// <summary>
        /// Retrieves a product by its ID, including its associated contents and attributes.
        /// </summary>
        /// <param name="productId">The ID of the product to retrieve.</param>
        /// <returns>
        /// A <see cref="Task{ProductEntity}"/> representing the asynchronous operation,
        /// containing the product entity if found; otherwise, null.
        /// </returns>
        /// <remarks>
        /// This method fetches the product along with its related Contents and Attributes entities from the database asynchronously.
        /// </remarks>
        Task<ProductEntity> GetProductById(long productId);

        /// <summary>
        /// Retrieves a product entity based on the provided checkout order details.
        /// </summary>
        /// <param name="order">A <see cref="CheckoutDTO"/> containing the product ID to checkout.</param>
        /// <returns>
        /// A <see cref="Task{ProductEntity}"/> representing the asynchronous operation,
        /// containing the product entity if found; otherwise, a new empty <see cref="ProductEntity"/>.
        /// </returns>
        /// <remarks>
        /// This method looks up the product by its ID from the checkout order.
        /// </remarks>
        Task<ProductEntity> Chekout(CheckoutDTO order);

        /// <summary>
        /// Checks if a product with the given ID is created within the internal system.
        /// </summary>
        /// <param name="productId">The ID of the product to check.</param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> indicating whether the product exists internally (true) or not (false).
        /// </returns>
        /// <remarks>
        /// This method queries the database for a product with the specified ID where the Provider field is empty,
        /// which indicates that the product was created internally rather than sourced externally.
        /// </remarks>
        Task<bool> CheckInternalSystemProduct(long productId);

        Task<ProductEntity?> GetExternalProductsWithOriginIdAsync(ProductDTO productDto);

    }
}
