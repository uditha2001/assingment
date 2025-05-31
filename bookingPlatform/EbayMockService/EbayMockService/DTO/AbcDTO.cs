using ProductService.API.DTO;

namespace AbcMockService.DTO
{
    public class AbcDTO
    {
        public long originId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string owner { get; set; }
        public decimal Price { get; set; }
        public int availableQuantity { get; set; } = 0;

        public string Currency { get; set; }
        public int ProductCategoryId { get; set; }
        public List<ProductAttributesDTO> Attributes { get; set; } = new();
        public List<ProductContentDTO> Contents { get; set; } = new();
    }

}
