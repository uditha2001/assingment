namespace OrderService.API.DTO
{
    public class OrderDTO
    {
        public long orderId {  get; set; }
        public long userId { get; set; }

        public decimal totalOrderprice { get; set; }

        public List<OrderItemsDTO> items { get; set; }

    }
}
