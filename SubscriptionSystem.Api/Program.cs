using SubscriptionSystem.Api;
using SubscriptionSystem.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var customers = new List<Customer>();


// Map a GET endpoint at "/" that prints to the console
app.MapGet("/", () =>
{

    Console.WriteLine("Hello, World!");
    return new Customer("Anna", "annawebster@example.com", "71 Pumpkin Hill Dr.Staten Island, NY 10314");

});

app.MapGet("/getCustomers", () => customers);

app.MapPost("/customers", (CreateCustomerDto dto) =>
{
    
    var customer = new Customer(dto.Name, dto.Email, dto.BillingAddress);
    customers.Add(customer);
    Console.WriteLine($"New customer created: {customer.Name}");
    return Results.Created($"/customers/{customer.GuId}", customer);
});

app.Run();