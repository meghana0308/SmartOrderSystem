using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartOrder.API.Models.Entities;

namespace SmartOrder.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);

            builder.Entity<Product>().Property(p => p.UnitPrice).HasPrecision(18, 2);
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(18, 2);
            builder.Entity<InvoiceItem>().Property(ii => ii.UnitPrice).HasPrecision(18, 2);
            builder.Entity<Invoice>().Property(i => i.TotalAmount).HasPrecision(18, 2);
            builder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);

            builder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasOne(o => o.CreatedByUser)
                .WithMany()
                .HasForeignKey(o => o.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            
            builder.Entity<Order>()
                .Property(o => o.PaymentMode)
                .HasConversion<string>()
                .IsRequired();
        }
    }
}
