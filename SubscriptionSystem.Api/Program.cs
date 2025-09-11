using SubscriptionSystem.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var customers = new List<Customer>();
var invoices = new List<Invoice>();
var subscriptions = new List<Subscription>();
var payments = new List<Payment>();

//dotnet run --urls "http://0.0.0.0:5034"
//app.MapLegacyEndpoints(customers);

app.MapCustomerEndpoints(customers);
app.MapSubscriptionEndpoints(customers, subscriptions);
app.MapPlanEndpoints();
app.MapInvoiceEndpoints(customers, invoices);
//app.MapPaymentEndpoints(payments);

app.Run();