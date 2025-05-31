namespace OrderService.API.DTO
{
    public class OrderItemsDTO
    {
        public long orderItemId {  get; set; }
        public long orderId { get; set; }
        public int quantity { get; set; }
        public long ProductId { get; set; }
        public decimal itemTotalPrice {  get; set; }

    }
}
