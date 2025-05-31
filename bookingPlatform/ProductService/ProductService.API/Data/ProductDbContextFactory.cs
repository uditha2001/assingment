using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductService.API.Data
{  public class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
        {
            public ProductDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();

                optionsBuilder.UseSqlServer("Data Source=CL-UDITHAI;Initial Catalog=Products;Integrated Security=True;Trust Server Certificate=True");

                return new ProductDbContext(optionsBuilder.Options);
            }
        }
}
