using Microsoft.EntityFrameworkCore;
using MyTodoList.Data.Models;

namespace MyTodoList.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) 
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<ToDoItem> ToDoItems { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.NormalizedUserName).IsUnique();
                entity.Property(u => u.UserName).HasMaxLength(256);
                entity.Property(u => u.NormalizedUserName).HasMaxLength(256);
            });
            modelBuilder.Entity<ToDoItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).HasMaxLength(256);
                entity.Property(t => t.UserId).HasMaxLength(256);
                entity.HasOne(t => t.User).WithMany(u => u.ToDoItems).HasForeignKey(t => t.UserId);
            });
        }
    }
}
