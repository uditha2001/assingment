using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartService.API.Model.Entities
{
    public class CartItemEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long cartItemId {  get; set; }
        [Required]
        public long UserId { get; set; }
        [Required]
        public long ProductId { get; set; }
        public int Quantity { get; set; } = 0;

        public decimal itemTotalPrice {  get; set; }

    }
}
