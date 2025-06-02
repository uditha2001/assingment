using OrderService.API.DTO;
using ProductService.API.DTO;
using ProductService.API.Models.Entities;

namespace ProductService.API.Services.serviceInterfaces
{
    /// <summary>
    /// Defines the bussinesss logics for product-related operations.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Maps a <see cref="ProductEntity"/> object to a <see cref="ProductDTO"/> object.
        /// NOTE: This method does not map the Attributes and Contents.
        /// </summary>
        /// <param name="productEntity">The product entity object to be mapped.</param>
        /// <returns>A <see cref="ProductDTO"/> object.</returns>
        public ProductDTO ProductEntityToDTO(ProductEntity productEntity);

        /// <summary>
        /// Maps a <see cref="ProductDTO"/> object to a <see cref="ProductEntity"/> object.
        /// NOTE: This method does not map the Attributes and Contents.
        /// </summary>
        /// <param name="productDTo">The product DTO object to be mapped.</param>
        /// <returns>A <see cref="ProductDTO"/> object.</returns>
        public ProductEntity ProductDTOToEntity(ProductDTO productDTo);

        /// <summary>
        /// Updates products by retrieving data from the external adapters and mapping it to internal product entities.all adapters will import data parallaly,thismethode auto maticaly run when project start.
        /// NOTE:all contents and attributes also imports
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation. The result contains a list of <see cref="ProductDTO"/> objects,came in external services or <c>null</c> if the update fails.
        /// </returns>
        Task<List<ProductDTO>?> UpdateProductsFromAdapterAsync();

        /// <summary>
        /// Retrieves all products, including those from internal and external services.
        /// Useful for displaying products in the frontend.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The result contains a list of <see cref="ProductDTO"/> objects.</returns>
        Task<List<ProductDTO>> GetAllProducts();


        /// <summary>
        /// Creates a new product asynchronously.
        /// </summary>
        /// <param name="productDto">The product data transfer object containing the product details.</param>
        /// <returns>
        /// A boolean value: returns <c>true</c> if the product was created successfully; otherwise, <c>false</c>.
        /// </returns>
        Task<long> CreateProduct(ProductDTO productdto);

        /// <summary>
        /// Deletes an existing product asynchronously.
        /// Note: Only products created internally by the system can be deleted.
        /// </summary>
        /// <param name="productId">The ID of the product to be deleted.</param>
        /// <returns>
        /// A boolean value: returns <c>true</c> if the product was deleted successfully; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteProductAsync(long productId);

        /// <summary>
        /// Retrieves all products created using the internal system.
        /// This is used to identify products that can be safely deleted or updated without affecting external systems.
        /// </summary>
        /// <returns>A list of <see cref="ProductDTO"/> objects.</returns>
        Task<List<ProductDTO>> GetInternalSystemProducts();

        /// <summary>
        /// Extracts attributes and content data from a <see cref="ProductDTO"/> object 
        /// and maps them to a given <see cref="ProductEntity"/>.
        /// </summary>
        /// <param name="productdto">
        /// The data transfer object containing product attributes and content information.
        /// </param>
        /// <param name="productentity">
        /// The entity object to which attributes and content data will be added.
        /// </param>
        /// <remarks>
        /// This method checks if the <c>Attributes</c> and <c>Contents</c> collections in the 
        /// <paramref name="productdto"/> are not null. If they exist, it iterates over each item 
        /// and maps the data to corresponding <see cref="ProductAttributesEntity"/> and 
        /// <see cref="ProductContentEntity"/> instances, which are then added to the 
        /// <paramref name="productentity"/>.
        /// </remarks>
        void ExtractAttributesAndContentFromProductDTO(ProductDTO productdto, ProductEntity productentity);

        /// <summary>
        /// Retrieves a list of products owned by a specific user and maps them to <see cref="ProductDTO"/> objects.
        /// </summary>
        /// <param name="userId">
        /// The ID of the user whose products are to be retrieved.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a list of <see cref="ProductDTO"/> 
        /// objects representing the user's products.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when the product retrieval process fails.
        /// </exception>
        /// <remarks>
        /// This method fetches all <see cref="ProductEntity"/> records associated with the given user ID using the product repository.
        /// Each entity is then converted into a <see cref="ProductDTO"/>, including mapping its attributes and content.
        /// </remarks>
        Task<List<ProductDTO>> GetOwnerProducts(long userId);

        /// <summary>
        /// Retrieves all product categories from the database and maps them to <see cref="ProductCategoryDTO"/> objects.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation, with a result containing a list of <see cref="ProductCategoryDTO"/> objects.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when the category retrieval process fails.
        /// </exception>
        Task<List<ProductCategoryDTO>> GetAllCategories();

        /// <summary>
        /// Processes the sale of one or more products based on the provided order data.
        /// It updates inventory for internal products and forwards external product sales to an adapter service.
        /// </summary>
        /// <param name="orderDto">
        /// A list of <see cref="CheckoutDTO"/> objects representing the products being sold and their quantities.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation, with a result indicating whether the sale was successful.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an unexpected error occurs during the sale process.
        /// </exception>
        /// <remarks>
        /// - If the product is internal and has sufficient quantity, the local inventory is updated.
        /// - If the product is external, the request is forwarded to an adapter service via HTTP.
        /// - If any order fails due to API or validation issues, the method returns <c>false</c>.
        /// </remarks>

        Task<bool> SellProducts(List<CheckoutDTO> orderDto);

        /// <summary>
        /// Retrieves an external product by its ID and maps it to a <see cref="ProductDTO"/>.
        /// </summary>
        /// <param name="productId">
        /// The ID of the external product to retrieve.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the <see cref="ProductDTO"/> if found; 
        /// otherwise, an empty <see cref="ProductDTO"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs during the retrieval process.
        /// </exception>
        /// <remarks>
        /// This method queries the repository for an external product using the provided ID. 
        /// If found, the product entity is converted to a DTO and returned. 
        /// If not found, an empty <see cref="ProductDTO"/> object is returned.
        /// </remarks>

        Task<ProductDTO> GetExtranalProductById(long productId);

        /// <summary>
        /// Retrieves a product by its ID, including both internal and external products, and maps it to a <see cref="ProductDTO"/>.
        /// </summary>
        /// <param name="productId">
        /// The ID of the product to retrieve.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the mapped <see cref="ProductDTO"/> if the product is found; 
        /// otherwise, an empty <see cref="ProductDTO"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs during the product retrieval process.
        /// </exception>
        /// <remarks>
        /// This method fetches a product using the given ID from the data repository. 
        /// It works for both internally managed and externally sourced products.
        /// If found, the product entity is mapped to a DTO including its attributes and content.
        /// If the product is not found, an empty DTO is returned.
        /// </remarks>

        Task<ProductDTO> GetProductById(long productId);

        /// <summary>
        /// Checks the availability of a product for checkout, handling both internal and external products.
        /// </summary>
        /// <param name="order">
        /// A <see cref="CheckoutDTO"/> object containing the product ID and quantity to be checked.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result is <c>true</c> if the product is available for checkout; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// - If the product is internal, the method checks its available quantity in the local database.
        /// - If the product is external, the method sends a request to an external adapter service to validate the checkout.
        /// - Returns <c>true</c> if the checkout is valid and the product is available; otherwise, returns <c>false</c>.
        /// </remarks>
        /// <exception cref="HttpRequestException">
        /// Thrown when the HTTP request to the external adapter fails.
        /// </exception>

        Task<bool> GetCheckout(CheckoutDTO order);

        /// <summary>
        /// Updates an existing product's details, attributes, and content using the provided <see cref="ProductDTO"/>.
        /// </summary>
        /// <param name="productDto">
        /// A <see cref="ProductDTO"/> containing the updated product data.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result is <c>true</c> if the product was found and successfully updated; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an unexpected error occurs during the update process.
        /// </exception>
        /// <remarks>
        /// This method performs the following:
        /// <list type="bullet">
        /// <item>Checks if the product exists using the provided DTO.</item>
        /// <item>If found, updates core product fields and timestamps.</item>
        /// <item>Removes all existing attributes and contents from the product (with specific conditions).</item>
        /// <item>Adds new attributes and contents from the provided DTO.</item>
        /// <item>Commits the changes to the database using the repository layer.</item>
        /// </list>
        /// If the product does not exist, the method returns <c>false</c>.
        /// </remarks>

        Task<bool> UpdateProduct(ProductDTO product);

    }
}
