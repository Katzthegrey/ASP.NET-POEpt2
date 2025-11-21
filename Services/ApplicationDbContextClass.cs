using Microsoft.EntityFrameworkCore; // Remove Identity.EntityFrameworkCore
using POEpt1.Models;

namespace POEpt1.Services
{
    public class ApplicationDbContextClass : DbContext // Change to regular DbContext
    {
        public ApplicationDbContextClass(DbContextOptions<ApplicationDbContextClass> options) : base(options)
        {
        }

        // Fix DbSet names (plural is conventional)
        public DbSet<User> Users { get; set; } // Change User → Users
        public DbSet<Role> Roles { get; set; } // Change Role → Roles
        public DbSet<Claim> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          

            // Seed initial roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleID = 1, RoleName = "Lecturer" },
                new Role { RoleID = 2, RoleName = "Coordinator" }, 
                new Role { RoleID = 3, RoleName = "Manager" },
                new Role { RoleID = 4, RoleName = "HR" }
            );

            // User-Role relationship
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Claims)
                      .WithOne(c => c.User)
                      .HasForeignKey(c => c.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Claim relationships
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Claims)
                      .HasForeignKey(c => c.UserID)
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.HasOne(c => c.Approver)
                      .WithMany()
                      .HasForeignKey(c => c.ApprovedBy)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);
            });

            // Optional: Add unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();
        }
    }
}