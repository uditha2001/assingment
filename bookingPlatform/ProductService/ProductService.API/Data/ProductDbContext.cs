using Microsoft.EntityFrameworkCore;
using ProductService.API.Models.Entities;

namespace ProductService.API.Data
{
    public class ProductDbContext: DbContext
    {
        public ProductDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductContentEntity> Content { get; set; }
        public DbSet<ProductAttributesEntity> productAttribute { get; set; }

        public DbSet<ProductCategoryEntity> productCategory { get; set; }

    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProductCategoryEntity>().HasData(
               new ProductCategoryEntity { Id = 1, Name = "Electronics", Description = "Devices and gadgets" },
               new ProductCategoryEntity { Id = 2, Name = "Books", Description = "Various types of books" },
               new ProductCategoryEntity { Id = 3, Name = "Clothing", Description = "Apparel and garments" },
               new ProductCategoryEntity { Id = 4, Name = "Home", Description = "Home and kitchen items" },
               new ProductCategoryEntity { Id = 5, Name = "Sports", Description = "Sports equipment and accessories" }
           );

            modelBuilder.Entity<ProductEntity>()
           .HasMany(p => p.Attributes)
           .WithOne(a => a.ProductEntity)
           .HasForeignKey(a => a.ProductId)
           .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ProductEntity>()
                .HasMany(p => p.Contents)
                .WithOne(c => c.Product)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProductEntity>()
           .HasOne(p => p.ProductCategory)
           .WithMany(c => c.Product)
           .HasForeignKey(p => p.ProductCategoryId);


        }



    }
}
