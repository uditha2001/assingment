using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.API.Models.Entity
{
    public class OrderItemEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long orderItemId { get; set; }
        [Required]
        public long orderId { get; set; }
        public int quantity { get; set; } = 0;
        [Required]
        public long ProductId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal itemTotalPrice { get; set; } = 0;

        [ForeignKey("orderId")]
        public OrderEntity orderEntity { get; set; }
    }
}
