using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserTask>Tasks {get;set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Define the relationship between AppUser and UserTask
    modelBuilder.Entity<UserTask>()
        .HasOne(ut => ut.User)  // Each UserTask has one AppUser
        .WithMany(au => au.Tasks)  // Each AppUser has many UserTasks
        .HasForeignKey(ut => ut.UserId)
        .IsRequired(false);
          // Foreign key in UserTask table
}

}
}