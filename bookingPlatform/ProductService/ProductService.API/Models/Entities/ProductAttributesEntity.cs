using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.API.Models.Entities
{
    [Table("productEntity")]
    public class ProductAttributesEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AttributeId { get; set; }  
        public String provider {  get; set; }=string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public long ProductId { get; set; }     
        public ProductEntity ProductEntity { get; set; }  
    }
}
