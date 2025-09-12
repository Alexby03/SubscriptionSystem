using SubscriptionSystem.Models;
using SubscriptionSystem.Data;
using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Services;

var builder = WebApplication.CreateBuilder(args);

//dotnet run --urls "http://0.0.0.0:5034"

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(9, 4, 0))
    )
);
builder.Services.AddScoped<SubscriptionService>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); //creates tables if they don’t exist
}

app.MapCustomerEndpoints();
app.MapSubscriptionEndpoints();
app.MapPlanEndpoints();
app.MapInvoiceEndpoints();
//app.MapPaymentEndpoints(payments);

app.Run();