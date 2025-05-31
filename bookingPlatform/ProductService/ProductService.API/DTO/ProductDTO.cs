using ProductService.API.Models.Entities;

namespace ProductService.API.DTO
{
    /// <summary>
    /// Data Transfer Object representing a Product entity in the Core Booking Platform.
    /// This DTO encapsulates all relevant product data used for API transmission,
    /// </summary>
    public class ProductDTO
    {
        public long Id { get; set; }              
        public string Name { get; set; }         
        public string Description { get; set; }
        public String owner { get; set; }
        public int availableQuantity { get; set; } = 0;
        public float rate { get; set; } = 0;
        public decimal Price { get; set; }        
        public string Currency { get; set; }     
        public long createdBy { get; set; }
        public int ProductCategoryId { get; set; }


        public List<ProductAttributesDTO> Attributes { get; set; } = new(); 
        public List<ProductContentDTO> Contents { get; set; } = new();
        public String provider { get; set; }=String.Empty;
        public long originId { get; set; } = -1;
    }
}
