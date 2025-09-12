using SubscriptionSystem.Models;
using SubscriptionSystem.Dtos;
using SubscriptionSystem.Results;
using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Data;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app)
    {
        //lists all customers
        app.MapGet("/customers", async (AppDbContext db) =>
        {
            var customers = await db.Customers.ToListAsync();
            return Results.Ok(customers);
        });

        //lists a customer by email
        app.MapGet("/customers/{email}", async (string email, AppDbContext db) =>
        {
            var customer = await db.Customers.FirstOrDefaultAsync(customer => customer.Email == email);
            if (customer == null)
                return Results.NotFound($"Customer with email {email} was not found.");

            return Results.Ok(customer);
        });

        //creates a new customer
        app.MapPost("/customers", async (CreateCustomerDto dto, AppDbContext db) =>
        {
            var customer = new Customer(dto.Name, dto.Email, dto.BillingAddress);
            try
            {
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
            } catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("Duplicate entry") == true)
            {
                return Results.Conflict($"Customer with email {dto.Email} already exists.");
            }
            return Results.Created($"/customers/{customer.CustomerId}", customer);
        });

        //replaces an existing customer info
        app.MapPut("/customers/{id}", async (Guid id, CreateCustomerDto dto, AppDbContext db) =>
        {
            var customer = await db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
                return Results.NotFound($"Customer with id {id} not found.");

            customer.Update(dto.Name, dto.Email, dto.BillingAddress);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Results.Conflict($"Could not update customer with id {id}. {ex.Message}");
            }
            return Results.Ok(customer);
        });

        // Deletes a customer
        app.MapDelete("/customers/{id}", async (Guid id, AppDbContext db) =>
        {
            var customer = await db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
                return Results.NotFound($"Customer with id {id} was not found.");
            try
            {
                db.Customers.Remove(customer);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Results.BadRequest($"Could not delete customer with id {id}. {ex.Message}");
            }
            return Results.NoContent();
        });

    }
}