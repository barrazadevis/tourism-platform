using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.Entities;

namespace TourismPlatform.Data
{
    public class TourismDbContext : DbContext
    {
        public TourismDbContext(DbContextOptions<TourismDbContext> options) : base(options) { }

        // Auth entities
        public DbSet<Application> Applications { get; set; }
        public DbSet<User> Users { get; set; }
        
        // Tourism entities
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TravelPlan> TravelPlans { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PlanService> PlanServices { get; set; }
        public DbSet<QuoteItem> QuoteItems { get; set; }
        public DbSet<QuoteHotel> QuoteHotels { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierService> SupplierServices { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<BookingMetric> BookingMetrics { get; set; }

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

            // User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20);

                entity.HasIndex(e => new { e.Email, e.TenantId }).IsUnique();

                entity.HasOne(u => u.Tenant)
                      .WithMany(t => t.Users)
                      .HasForeignKey(u => u.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.Application)
                      .WithMany(a => a.Users)
                      .HasForeignKey(u => u.ApplicationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Tenant entity
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Subdomain).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PlanType).IsRequired().HasMaxLength(20);

                entity.HasIndex(e => e.Subdomain).IsUnique();

                entity.HasOne(t => t.Application)
                      .WithMany(a => a.Tenants)
                      .HasForeignKey(t => t.ApplicationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Customer configurations
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.DocumentNumber);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.DocumentType).HasMaxLength(50);
                entity.Property(e => e.DocumentNumber).HasMaxLength(50);

                entity.HasOne(c => c.Tenant)
                      .WithMany()
                      .HasForeignKey(c => c.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TravelPlan entity
            modelBuilder.Entity<TravelPlan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Destination).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PlanType).HasMaxLength(100);
                entity.Property(e => e.BasePrice).HasColumnType("decimal(18,2)");

                entity.HasOne(tp => tp.Tenant)
                      .WithMany(t => t.TravelPlans)
                      .HasForeignKey(tp => tp.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Quote entity
            modelBuilder.Entity<Quote>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.QuoteNumber).IsUnique();
                entity.Property(e => e.QuoteNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PricePerPerson).HasColumnType("decimal(18,2)");

                entity.HasOne(q => q.Customer)
                      .WithMany(c => c.Quotes)
                      .HasForeignKey(q => q.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(q => q.TravelPlan)
                      .WithMany(tp => tp.Quotes)
                      .HasForeignKey(q => q.TravelPlanId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(q => q.Tenant)
                      .WithMany(t => t.Quotes)
                      .HasForeignKey(q => q.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // QuoteItem configurations
            modelBuilder.Entity<QuoteItem>(entity =>
            {
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ItemType).HasMaxLength(100);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");

                entity.HasOne(qi => qi.Quote)
                      .WithMany(q => q.Items)
                      .HasForeignKey(qi => qi.QuoteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // QuoteHotel configurations
            modelBuilder.Entity<QuoteHotel>(entity =>
            {
                entity.Property(e => e.HotelName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.HotelCategory).HasMaxLength(100);
                entity.Property(e => e.RoomType).HasMaxLength(100);
                entity.Property(e => e.PlanType).HasMaxLength(100);
                entity.Property(e => e.RoomPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxesPrice).HasColumnType("decimal(18,2)");

                entity.HasOne(qh => qh.Quote)
                      .WithMany(q => q.Hotels)
                      .HasForeignKey(qh => qh.QuoteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Booking configurations
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasIndex(e => e.BookingNumber).IsUnique();
                entity.Property(e => e.BookingNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalPaid).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PendingAmount).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.Quote)
                    .WithMany(q => q.Bookings)
                    .HasForeignKey(e => e.QuoteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Bookings)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Tenant)
                    .WithMany()
                    .HasForeignKey(e => e.TenantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Passenger configurations
            modelBuilder.Entity<Passenger>(entity =>
            {
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DocumentType).HasMaxLength(50);
                entity.Property(e => e.DocumentNumber).HasMaxLength(50);
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.Nationality).HasMaxLength(100);

                entity.HasOne(p => p.Booking)
                      .WithMany(b => b.Passengers)
                      .HasForeignKey(p => p.BookingId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Customer)
                      .WithMany(c => c.Passengers)
                      .HasForeignKey(p => p.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Payment configurations
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasIndex(e => e.PaymentNumber).IsUnique();
                entity.Property(e => e.PaymentNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PaymentMethod).HasMaxLength(100);
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.TransactionId).HasMaxLength(200);

                entity.HasOne(p => p.Booking)
                      .WithMany(b => b.Payments)
                      .HasForeignKey(p => p.BookingId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Tenant)
                      .WithMany()
                      .HasForeignKey(p => p.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Supplier configurations
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ContactEmail).HasMaxLength(255);
                entity.Property(e => e.ContactPhone).HasMaxLength(20);
                entity.Property(e => e.SupplierType).HasMaxLength(100);

                entity.HasOne(s => s.Tenant)
                      .WithMany()
                      .HasForeignKey(s => s.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SupplierService configurations
            modelBuilder.Entity<SupplierService>(entity =>
            {
                entity.Property(e => e.ServiceType).HasMaxLength(100);
                entity.Property(e => e.ServiceName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Cost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).HasMaxLength(3);

                entity.HasOne(ss => ss.Supplier)
                      .WithMany(s => s.Services)
                      .HasForeignKey(ss => ss.SupplierId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Document configurations
            modelBuilder.Entity<Document>(entity =>
            {
                entity.Property(e => e.DocumentType).HasMaxLength(100);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FileExtension).HasMaxLength(10);
                entity.Property(e => e.UploadedBy).HasMaxLength(100);

                entity.HasOne(d => d.Booking)
                      .WithMany(b => b.Documents)
                      .HasForeignKey(d => d.BookingId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Customer)
                      .WithMany(c => c.Documents)
                      .HasForeignKey(d => d.CustomerId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Tenant)
                      .WithMany()
                      .HasForeignKey(d => d.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // BookingMetric configurations
            modelBuilder.Entity<BookingMetric>(entity =>
            {
                entity.Property(e => e.TotalRevenue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AverageBookingValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Period).HasMaxLength(50);

                entity.HasOne(bm => bm.Tenant)
                      .WithMany()
                      .HasForeignKey(bm => bm.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // PlanService configurations
            modelBuilder.Entity<PlanService>(entity =>
            {
                entity.Property(e => e.ServiceType).HasMaxLength(100);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                entity.HasOne(ps => ps.TravelPlan)
                      .WithMany(tp => tp.Services)
                      .HasForeignKey(ps => ps.TravelPlanId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Enum conversions
            modelBuilder.Entity<Quote>()
                .Property(e => e.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Booking>()
                .Property(e => e.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Booking>()
                .Property(e => e.PaymentStatus)
                .HasConversion<int>();

            modelBuilder.Entity<Payment>()
                .Property(e => e.PaymentStatus)
                .HasConversion<int>();
        }
    }
}