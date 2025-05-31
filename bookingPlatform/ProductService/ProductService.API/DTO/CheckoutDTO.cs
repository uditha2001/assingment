namespace OrderService.API.DTO
{
    public class CheckoutDTO
    {
        public int quantity { get; set; }
        public long ProductId { get; set; }
        public decimal itemTotalPrice {  get; set; }

    }
}
