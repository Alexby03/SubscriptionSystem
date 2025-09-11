using SubscriptionSystem.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var customers = new List<Customer>();
var invoices = new List<Invoice>();

//dotnet run --urls "http://0.0.0.0:5034"
//app.MapLegacyEndpoints(customers);

app.MapCustomerEndpoints(customers);


app.Run();