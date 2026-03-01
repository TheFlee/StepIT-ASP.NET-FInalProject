using InvoiceManagerAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagerAPI.Data;

public class InvoiceManagerDbContext : IdentityDbContext<User>
{
    public InvoiceManagerDbContext(DbContextOptions<InvoiceManagerDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<TokenSession> TokenSessions => Set<TokenSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade); 
        });

        // Invoice
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired();
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.Status).IsRequired();

            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Invoices)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
