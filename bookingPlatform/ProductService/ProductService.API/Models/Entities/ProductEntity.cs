using Microsoft.AspNetCore.Mvc;
using ProductService.API.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.API.Models.Entities
{
    public class ProductEntity
    {
        [Key]
        public long Id { get; set; }
        public long originId { get; set; } = -1;

        [MaxLength(100)]
        public string Provider { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        public String owner {  get; set; }
        public int availableQuantity { get; set; } = 0;


        public float rate { get; set; } = 0;


        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [ForeignKey("ProductCategoryId")]
        public ProductCategoryEntity ProductCategory { get; set; }
        public int ProductCategoryId { get; set; }

        [Required]
        public string Currency { get; set; } = "USD";
        public List<ProductAttributesEntity> Attributes { get; set; } = new();
        public List<ProductContentEntity> Contents { get; set; } = new();

        public long createdBy {  get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
