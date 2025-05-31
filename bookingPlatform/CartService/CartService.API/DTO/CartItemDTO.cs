namespace CartService.API.DTO
{
    public class CartItemDTO
    {
        public long? cartItemId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public long userId {  get; set; }
        public decimal itemTotalPrice { get; set; }



    }
}
