using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Entities;
using SubscriptionSystem.Outbox;

namespace SubscriptionSystem.Data;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<OutboxEvent> OutboxEvents { get; set; }


    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //ENTITY RELATIONS:

        //CUSTOMER
        //unique email constraint
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique();

        //SUBSCRIPTION
        modelBuilder.Entity<Subscription>()
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        //INVOICE
        modelBuilder.Entity<Invoice>()
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Subscription)      
            .WithMany()                       
            .HasForeignKey(i => i.SubscriptionId) 
            .OnDelete(DeleteBehavior.SetNull);   

        //PAYMENT
        modelBuilder.Entity<Payment>()
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .HasOne<Invoice>()
            .WithMany()
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
