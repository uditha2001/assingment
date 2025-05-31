using ProductService.API.DTO;

namespace CdeMockService.DTO
{
    public class CdeDTO
    {
        public long originId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public String owner { get; set; }
        public int availableQuantity { get; set; } = 0;

        public float rate { get; set; } = 0;
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int ProductCategoryId { get; set; }
        public List<ProductAttributesDTO> Attributes { get; set; } = new();
        public List<ProductContentDTO> Contents { get; set; } = new();
    }
}
