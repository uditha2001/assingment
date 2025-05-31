using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.API.Models.Entity
{
    public class OrderEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long orderId { get; set; }
        [Required]
        public long userId { get; set; }
        public DateTime? createdDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]

        public decimal totalOrderprice { get; set; } = 0;

        public List<OrderItemEntity> items { get; set; }
    }
}
