using ProductService.API.Models.Entities;

namespace ProductService.API.Repository.RepositoryInterfaces
{
    public interface IProductAttriuteRepository
    {
        /// <summary>
        /// Retrieves all product attributes associated with a specific product.
        /// </summary>
        /// <param name="productId">The product ID to filter attributes.</param>
        /// <returns>List of <see cref="ProductAttributesEntity"/> linked to the given product ID.</returns>
        Task<List<ProductAttributesEntity>> GetAllAttributes(long productId);

        /// <summary>
        /// Deletes a product attribute only for internal service created.
        /// </summary>
        /// <param name="attributeId">The ID of the attribute to delete.</param>
        /// <returns>True if the attribute is successfully deleted; otherwise, false.</returns>
        Task<bool> DeleteAttribute(long attributeId);

        /// <summary>
        /// Updates attribute lst which created usiginternal system.
        /// </summary>
        /// <param name="attributeList">List of updated <see cref="ProductAttributesEntity"/>.</param>
        /// <returns>True if update is successful for at least one attribute; otherwise, false.</returns>
        Task<bool> UpdateAttributesAsync(List<ProductAttributesEntity> attributeList);

        /// <summary>
        /// Creates multiple product attributes in the database.
        /// </summary>
        /// <param name="contents">List of <see cref="ProductAttributesEntity"/> to be added.</param>
        /// <returns>True if attributes are successfully created; otherwise, false.</returns>
        Task<bool> CreateAttributes(List<ProductAttributesEntity> contents);
    }
}
