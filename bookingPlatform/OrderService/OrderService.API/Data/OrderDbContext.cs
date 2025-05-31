using Microsoft.EntityFrameworkCore;
using OrderService.API.Models.Entity;

namespace OrderService.API.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<OrderEntity> Order {  get; set; }   
        public DbSet<OrderItemEntity> orderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<OrderEntity>()
           .HasMany(o => o.items)
           .WithOne(a => a.orderEntity)
           .HasForeignKey(a => a.orderId)
           .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
