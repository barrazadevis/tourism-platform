using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.Entities;
using System.Text.Json;

namespace TourismPlatform.Data;

public class TourismDbContext : DbContext
{
    public TourismDbContext(DbContextOptions<TourismDbContext> options) : base(options) { }

        // Auth entities (movidas desde Auth service)
        public DbSet<Application> Applications { get; set; }
        public DbSet<User> Users { get; set; }
        
        // Tourism entities (existentes)
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TravelPlan> TravelPlans { get; set; }
        public DbSet<Quote> Quotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Application entity
            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // User entity (con tenant relationship)
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
                
                entity.HasIndex(e => new { e.Email, e.TenantId }).IsUnique();
                
                // Relationship con Tenant
                entity.HasOne(u => u.Tenant)
                      .WithMany(t => t.Users)
                      .HasForeignKey(u => u.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relationship con Application
                entity.HasOne(u => u.Application)
                      .WithMany(a => a.Users)
                      .HasForeignKey(u => u.ApplicationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Tenant entity (actualizada)
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Subdomain).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PlanType).IsRequired().HasMaxLength(20);
                
                entity.HasIndex(e => e.Subdomain).IsUnique();
                
                // Relationship con Application
                entity.HasOne(t => t.Application)
                      .WithMany(a => a.Tenants)
                      .HasForeignKey(t => t.ApplicationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TravelPlan entity (sin cambios)
            modelBuilder.Entity<TravelPlan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.BasePrice).HasColumnType("decimal(18,2)");
                
                entity.HasOne(tp => tp.Tenant)
                      .WithMany(t => t.TravelPlans)
                      .HasForeignKey(tp => tp.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Quote entity (sin cambios)
            modelBuilder.Entity<Quote>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                
                entity.HasOne(q => q.TravelPlan)
                      .WithMany(tp => tp.Quotes)
                      .HasForeignKey(q => q.TravelPlanId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(q => q.Tenant)
                      .WithMany(t => t.Quotes)
                      .HasForeignKey(q => q.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }