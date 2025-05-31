using ProductService.API.DTO;
using ProductService.API.Models.Entities;

namespace ProductService.API.Services.serviceInterfaces
{
    /// <summary>
    /// Service implementation for managing product attributes.
    /// Handles operations like creating, updating, retrieving, and deleting product attributes.
    /// </summary>
    public interface IProductAttributeService
    {
        /// <summary>
        /// Creates a list of product attributes and associates them with a product.
        /// </summary>
        /// <param name="productAttributesDTOs">List of attribute DTOs to be saved.</param>
        /// <param name="productId">ID of the product to associate attributes with.</param>
        /// <returns>True if creation is successful; otherwise, false.</returns>
        Task<bool> createAttribute(List<ProductAttributesDTO> productAttributesDTOs,long productId);

        /// <summary>
        /// Updates a list of product attributes.
        /// </summary>
        /// <param name="productAttributeDTOs">List of updated product attribute DTOs.</param>
        /// <param name="productId">ID of the product to which these attributes belong.</param>
        /// <returns>True if update was successful; otherwise, false.</returns>
        Task<bool> updateAttribute(List<ProductAttributesDTO> productAttributeDTOs,long productId);

        /// <summary>
        /// Deletes a product attribute by its ID if the provider is empty.
        /// </summary>
        /// <param name="attributeId">ID of the attribute to delete.</param>
        /// <returns>True if deletion was successful; otherwise, false.</returns>
        Task<bool> deleteAttribute(long attributeId);


        /// <summary>
        /// Retrieves all attributes associated with a given product ID.
        /// </summary>
        /// <param name="productId">The product ID for which attributes are to be fetched.</param>
        /// <returns>List of <see cref="ProductAttributesDTO"/>.</returns>
        Task<List<ProductAttributesDTO>> getAllAttributes(long ProductId);

        /// <summary>
        /// Maps a <see cref="ProductAttributesDTO"/> to a <see cref="ProductAttributesEntity"/>.
        /// </summary>
        /// <param name="dto">The DTO to map.</param>
        /// <param name="productId">The ID of the associated product.</param>
        /// <returns>A new <see cref="ProductAttributesEntity"/> instance.</returns>
        ProductAttributesEntity ToEntity(ProductAttributesDTO dto, long productId);

        /// <summary>
        /// Maps a <see cref="ProductAttributesEntity"/> to a <see cref="ProductAttributesDTO"/>.
        /// </summary>
        /// <param name="entity">The entity to map.</param>
        /// <returns>A new <see cref="ProductAttributesDTO"/> instance.</returns>
        ProductAttributesDTO ToDTO(ProductAttributesEntity entity);




    }
}
