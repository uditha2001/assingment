namespace CartService.API.Model
    {
    internal class CartItemEntity : Entities.CartItemEntity
        {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotalPrice { get; set; }
        }
    }