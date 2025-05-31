using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.API.Models.Entities
{
    public class ProductCategoryEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  

        public string Name { get; set; }  

        public string Description { get; set; }
        public List<ProductEntity> Product { get; set; }
    }
}
