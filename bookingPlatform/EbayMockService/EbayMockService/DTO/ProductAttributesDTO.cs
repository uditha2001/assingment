namespace ProductService.API.DTO
{
    /// <summary>
    /// Data Transfer Object representing a key-value pair of attribute metadata
    /// associated with a specific product instance. Attributes enable extended
    /// semantic tagging of products, useful for filtering, search, and UI display.
    /// </summary>
    public class ProductAttributesDTO
    {
        public long attributeId { get; set; }

        public string Key { get; set; }     
        public string Value { get; set; }
    }
}
