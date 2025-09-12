using SubscriptionSystem.Models;
using SubscriptionSystem.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//var customers = new List<Customer>();
//var invoices = new List<Invoice>();
//var subscriptions = new List<Subscription>();
//var payments = new List<Payment>();

//dotnet run --urls "http://0.0.0.0:5034"
//app.MapLegacyEndpoints(customers);

// Add DB context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(9, 4, 0))
    )
);

var app = builder.Build();

// You don’t need the in-memory lists anymore
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // creates tables if they don’t exist
}

app.MapCustomerEndpoints();
//app.MapSubscriptionEndpoints();
//app.MapPlanEndpoints();
//app.MapInvoiceEndpoints();
//app.MapPaymentEndpoints(payments);

app.Run();