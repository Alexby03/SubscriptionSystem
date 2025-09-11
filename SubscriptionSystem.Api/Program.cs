using SubscriptionSystem.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var customers = new List<Customer>();
var invoices = new List<Invoice>();
var subscriptions = new List<Subscription>();

//dotnet run --urls "http://0.0.0.0:5034"
//app.MapLegacyEndpoints(customers);

app.MapCustomerEndpoints(customers);
app.MapSubscriptionEndpoints(customers, subscriptions, invoices);

app.Run();