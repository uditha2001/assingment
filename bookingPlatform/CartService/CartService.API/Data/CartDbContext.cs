using CartService.API.DTO;
using CartService.API.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartService.API.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options)
        {
        }

        public DbSet<CartItemEntity> cartItems { get; set; }
    }
}
