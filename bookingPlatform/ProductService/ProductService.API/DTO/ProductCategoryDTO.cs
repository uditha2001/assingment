namespace ProductService.API.DTO
{
    public class ProductCategoryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
