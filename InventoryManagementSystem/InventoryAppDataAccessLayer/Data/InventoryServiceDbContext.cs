using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDomainLayer.DataModels.AuthenticationModels;
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryAppDataAccessLayer.Data
{
    public class InventoryServiceDbContext : DbContext
    {
        public DbSet<UserRegistrationDetails> UserRegistration { get; set; }
        public DbSet<DashboardFeaturePanel> FeaturePanels { get; set; }
        public DbSet<Category> Categories { get; set; }


        private Guid id;
        public InventoryServiceDbContext(DbContextOptions<InventoryServiceDbContext> options) : base(options)
        {
            id = Guid.NewGuid();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRegistrationDetails>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired().HasColumnType("uniqueidentifier");

                entity.Property(e => e.UserName).IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");

                entity.Property(e => e.Email).IsRequired().HasColumnType("uniqueidentifier").HasMaxLength(256).HasColumnType("nvarchar(256)");

                entity.Property(e => e.PasswordHash).IsRequired().HasColumnType("nvarchar(max)");

                entity.Property(e => e.ResetPasswordOTP).HasColumnType("varchar(10)");

                entity.Property(e => e.OTPExpiration).HasColumnType("datetime2");

                entity.Property(e => e.Role).IsRequired().HasMaxLength(50).HasColumnType("nvarchar(50)");

                entity.Property(e => e.EmailConfirmed).HasColumnType("bit");

                entity.Property(e => e.EmailConfirmToken).HasMaxLength(800).HasColumnType("uniqueidentifier").HasColumnType("nvarchar(800)");

                entity.Property(e => e.LastLogin).IsRequired().HasColumnType("datetime2");

                entity.Property(e => e.IsActive).IsRequired().HasColumnType("bit");

                entity.Property(e => e.SecurityStamp).HasMaxLength(128).HasColumnType("nvarchar(128)");

                entity.Property(e => e.PhoneNumber).IsRequired().HasColumnType("uniqueidentifier").HasMaxLength(20).HasColumnType("nvarchar(20)");

                entity.Property(e => e.PhoneNumberConfirmed).IsRequired().HasColumnType("bit");

                entity.Property(e => e.TwoFactorEnabled).IsRequired().HasColumnType("bit");

                entity.Property(e => e.AccessFailedCount).IsRequired().HasColumnType("int");

                entity.Property(e => e.RefreshToken).HasMaxLength(128).HasColumnType("nvarchar(128)");

                entity.Property(e => e.TokenExpiration).HasColumnType("datetime2");

                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime2");
            });

            modelBuilder.Entity<DashboardFeaturePanel>(entity =>
            {
                entity.HasKey(e => e.FeatureId);

                entity.Property(e => e.FeatureName).IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");

                entity.Property(e => e.FeatureViewKey).IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories"); // Table name

                // Primary Key
                entity.HasKey(e => e.CategoryId);

                // Column Configuration
                entity.Property(e => e.CategoryId)
                      .IsRequired()
                      .HasColumnType("int")
                      .ValueGeneratedOnAdd(); // Auto-increment

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnType("nvarchar(100)");

                entity.Property(e => e.Abbreviation)
                      .HasMaxLength(20)
                      .HasColumnType("nvarchar(20)");

                entity.Property(e => e.Level)
                      .IsRequired()
                      .HasColumnType("int");

                entity.Property(e => e.ParentCategoryId)
                      .HasColumnType("int")
                      .IsRequired(false); // Nullable for parent category

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                // Self-referencing foreign key relationship
                entity.HasOne(e => e.Parent)
                      .WithMany(e => e.SubCategories)
                      .HasForeignKey(e => e.ParentCategoryId)
                      .OnDelete(DeleteBehavior.Restrict) // Or Cascade, depending on the behavior you need
                      .HasConstraintName("FK_Categories_Parent"); // Foreign key constraint name
            });



            base.OnModelCreating(modelBuilder);
            var initializer = new DbInitializer(modelBuilder);
            initializer.Seed();
        }

    }
}
