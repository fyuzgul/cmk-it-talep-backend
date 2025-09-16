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
        public DbSet<RequestStatus> RequestStatuses { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestResponse> RequestResponses { get; set; }
        public DbSet<MessageReadStatus> MessageReadStatuses { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

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

            // RequestStatus entity configuration
            modelBuilder.Entity<RequestStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                
                // Global query filter for soft delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });


            // Request entity configuration
            modelBuilder.Entity<Request>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.SupportProviderId).IsRequired(false);
                entity.Property(e => e.RequestCreatorId).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.ScreenshotFilePath).HasMaxLength(500);
                entity.Property(e => e.RequestStatusId).IsRequired();
                entity.Property(e => e.RequestTypeId).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");

                // Foreign Key Relationships
                entity.HasOne(r => r.SupportProvider)
                      .WithMany()
                      .HasForeignKey(r => r.SupportProviderId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.RequestCreator)
                      .WithMany()
                      .HasForeignKey(r => r.RequestCreatorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.RequestStatus)
                      .WithMany()
                      .HasForeignKey(r => r.RequestStatusId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.RequestType)
                      .WithMany()
                      .HasForeignKey(r => r.RequestTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Global query filter for soft delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // RequestResponse entity configuration
            modelBuilder.Entity<RequestResponse>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.FilePath).HasMaxLength(500);
                entity.Property(e => e.RequestId).IsRequired();
                entity.Property(e => e.SenderId).IsRequired(false);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");

                // Foreign Key Relationships
                entity.HasOne(r => r.Request)
                      .WithMany(req => req.RequestResponses)
                      .HasForeignKey(r => r.RequestId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Sender)
                      .WithMany()
                      .HasForeignKey(r => r.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Global query filter for soft delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // MessageReadStatus entity configuration
            modelBuilder.Entity<MessageReadStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.MessageId).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.ReadAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");

                // Foreign Key Relationships
                entity.HasOne(mrs => mrs.Message)
                      .WithMany(rr => rr.ReadStatuses)
                      .HasForeignKey(mrs => mrs.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mrs => mrs.User)
                      .WithMany(u => u.MessageReadStatuses)
                      .HasForeignKey(mrs => mrs.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint to prevent duplicate read statuses
                entity.HasIndex(e => new { e.MessageId, e.UserId }).IsUnique();

                // Global query filter for soft delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });


            // PasswordResetToken entity configuration
            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ExpiresAt).IsRequired();
                entity.Property(e => e.IsUsed).HasDefaultValue(false);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");

                // Index for faster lookups
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasIndex(e => e.Email);

                // Global query filter for soft delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }
    }
}
