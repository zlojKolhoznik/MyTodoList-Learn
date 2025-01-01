using Microsoft.EntityFrameworkCore;

namespace MyTodoList.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) 
        {

        }

        public DbSet<User> Users { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.NormalizedUserName).IsUnique();
                entity.Property(u => u.UserName).HasMaxLength(256);
                entity.Property(u => u.NormalizedUserName).HasMaxLength(256);
            });
        }
    }
}
