using SubscriptionSystem.Models;
using SubscriptionSystem.Dtos;
using SubscriptionSystem.Results;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app, List<Customer> customers)
    {
        //lists all customers
        app.MapGet("/customers", () => Results.Ok(customers));

        //lists a customer by email
        app.MapGet("/customers/{email}", (string email) =>
        {
            var customer = customers.FirstOrDefault(customer => customer.Email == email);
            if (customer == null)
                return Results.NotFound($"Customer with email {email} was not found.");

            return Results.Ok(customer);
        });

        //creates a new customer
        app.MapPost("/customers", (CreateCustomerDto dto) =>
        {
            var dupCustomer = customers.FirstOrDefault(c => c.Email == dto.Email);
            if (dupCustomer != null)
                return Results.Conflict($"Customer with email {dto.Email} already exists.");
            var customer = new Customer(dto.Name, dto.Email, dto.BillingAddress);
            customers.Add(customer);
            return Results.Created($"/customers/{customer.CustomerId}", customer);
        });

        //replaces an existing customer info
        app.MapPut("/customers/{id}", (Guid id, CreateCustomerDto dto) =>
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
                return Results.NotFound($"Customer with email {id} was not found.");
            if (customer.Update(dto.Name, dto.Email, dto.BillingAddress) == GenericResult.Success)
            {
                return Results.Ok(customer);
            }
            return Results.BadRequest("Failed to update customer.");
        });

        //deletes customer
        app.MapDelete("/customers/{id}", (Guid id) =>
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
                return Results.NotFound($"Customer with id {id} was not found.");
            customers.Remove(customer);
            return Results.NoContent();
        });
    }
}