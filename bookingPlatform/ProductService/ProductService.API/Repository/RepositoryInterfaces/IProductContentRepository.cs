using ProductService.API.DTO;
using ProductService.API.Models.Entities;

namespace ProductService.API.Repository.RepositoryInterfaces
{
    public interface IProductContentRepository
    {
        /// <summary>
        /// Creates a new product content entity asynchronously.
        /// </summary>
        /// <param name="productContentEntity">The <see cref="ProductContentEntity"/> to add.</param>
        /// <returns>A task that represents the asynchronous operation, returning <c>true</c> upon successful creation.</returns>
        Task<bool> CreateContentAsync(ProductContentEntity productContentEntity);

        /// <summary>
        /// Updates an existing product content entity asynchronously.
        /// Only content created using internal system can update
        /// </summary>
        /// <param name="updatedContent">The <see cref="ProductContentEntity"/> containing the updated values.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// Returns <c>true</c> if the update was successful; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> UpdateContentAsync(ProductContentEntity updatedContent);

        /// <summary>
        /// Deletes a product content entry by its ID asynchronously.
        /// Only content created by the internal system (i.e., with an empty provider) will be deleted.
        /// </summary>
        /// <param name="contentId">The unique identifier of the content to delete.</param>
        /// <returns>
        /// A task representing the asynchronous operation. 
        /// Returns <c>true</c> if the content was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteContentByIdAsync(long contentId);

        /// <summary>
        /// Retrieves all content entries associated with a specific product ID asynchronously.
        /// </summary>
        /// <param name="productId">The ID of the product for which to retrieve content.</param>
        /// <returns>
        /// A task representing the asynchronous operation. 
        /// The task result contains a list of <see cref="ProductContentEntity"/> objects associated with the product.
        /// </returns>
        Task<List<ProductContentEntity>> GetAllContentAsync(long productId);

        Task<ProductContentEntity> GetContentByIdAsync(long id);

    }
}
