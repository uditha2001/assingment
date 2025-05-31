namespace ProductService.API.DTO
{
    /// <summary>
    /// Data Transfer Object representing media content associated with a product.

    /// </summary>
    public class ProductContentDTO
    {
        public long contentId { get; set; }
        public String provider {  get; set; }
        public string Type { get; set; }          
        public string Url { get; set; }           
        public string Description { get; set; }
    }
}
