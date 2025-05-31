using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.API.Models.Entities
{
    [Table("ProductContents")]
    public class ProductContentEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ContentId { get; set; }
        public String provider { get; set; } = string.Empty;


        [Required]
        public string Type { get; set; }

        [Required]
        public string Url { get; set; }

        public string Description { get; set; }

        public long ProductId { get; set; }

        public ProductEntity Product { get; set; }
    }
}
