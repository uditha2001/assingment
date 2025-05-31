using Microsoft.EntityFrameworkCore;
using userService.Api.entity;

namespace userService.Api.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }
        public DbSet<UserEntity> Users { get; set; }
    }
}
