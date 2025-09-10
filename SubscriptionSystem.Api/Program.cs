using SubscriptionSystem.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var customers = new List<Customer>();
var invoices = new List<Invoice>();

//dotnet run --urls "http://0.0.0.0:5034"
//TODO: Fix Result classes return types and remove all the checks in Endpoints.cs where you can

app.MapCustomerEndpoints(customers);

app.Run();