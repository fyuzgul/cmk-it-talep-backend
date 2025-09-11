using Microsoft.EntityFrameworkCore;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<SupportType> SupportTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Department entity configuration
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                
                // Global query filter for soft delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // UserType entity configuration
            modelBuilder.Entity<UserType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                
                // Global query filter for soft delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                
                // Foreign Key Relationships
                entity.HasOne(u => u.Department)
                      .WithMany()
                      .HasForeignKey(u => u.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.UserType)
                      .WithMany()
                      .HasForeignKey(u => u.TypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint for email
                entity.HasIndex(e => e.Email).IsUnique();
                
                // Global query filter for soft delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // SupportType entity configuration
            modelBuilder.Entity<SupportType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                
                // Global query filter for soft delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // RequestType entity configuration
            modelBuilder.Entity<RequestType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.SupportTypeId).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");

                // Foreign Key Relationship to SupportType
                entity.HasOne(r => r.SupportType)
                      .WithMany(s => s.RequestTypes)
                      .HasForeignKey(r => r.SupportTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Global query filter for soft delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }
    }
}
