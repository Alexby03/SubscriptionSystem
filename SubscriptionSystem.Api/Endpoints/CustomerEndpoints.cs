using SubscriptionSystem.Entities;
using SubscriptionSystem.Dtos;
using SubscriptionSystem.Results;
using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Data;
using SubscriptionSystem.Services;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app)
    {
        //lists all customers
        app.MapGet("/customers", async (CustomerService service) =>
        {
            var customers = await service.GetAllCustomersAsync();
            if (customers.Count == 0)
                return Results.NotFound("No customers in database.");
            return Results.Ok(customers);
        });

        //lists a customer by email
        app.MapGet("/customers/{email}", async (string email, CustomerService service) =>
        {
            var customer = await service.GetCustomerByEmailAsync(email);
            if (customer == null)
                return Results.NotFound($"Customer with email {email} was not found.");

            return Results.Ok(customer);
        });

        //creates a new customer
        app.MapPost("/customers", async (CreateCustomerDto dto, CustomerService service) =>
        {
            var result = await service.CreateCustomerAsync(dto);
            if (result == GenericResult.Duplicate)
                return Results.Conflict($"Customer with email {dto.Email} already exists.");
            return Results.Created($"Successfully created customer with email {dto.Email}.", dto);
        });

        //replaces an existing customer info
        app.MapPut("/customers/{id}", async (Guid id, CreateCustomerDto dto, CustomerService service) =>
        {
            var customer = await service.UpdateCustomerAsync(id, dto);
            if (customer == null)
                return Results.NotFound($"Customer with id {id} not found.");
            return Results.Ok(customer);
        });

        //mark customer inactive
        app.MapPut("/customers/{id}/markInactive", async (Guid id, CustomerService service) =>
        {
            var customer = await service.MarkCustomerInactiveAsync(id);
            if (customer == null)
                return Results.NotFound($"Customer with id {id} not found.");
            return Results.Ok(customer);
        });
    }
}