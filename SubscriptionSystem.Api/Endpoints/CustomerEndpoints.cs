using SubscriptionSystem.Models;
using SubscriptionSystem.Dtos;
using SubscriptionSystem.Results;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app, List<Customer> customers)
    {
        //lists all customers
        app.MapGet("/customers", () => customers);

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
            return Results.Created($"/customers/{customer.GuId}", customer);
        });

        //replaces an existing customer info
        app.MapPut("/customers/{id}", (Guid id, CreateCustomerDto dto) =>
        {
            var customer = customers.FirstOrDefault(c => c.GuId == id);
            if (customer == null)
                return Results.NotFound($"Customer with email {id} was not found.");
            customer.Update(dto.Name, dto.Email, dto.BillingAddress);
            return Results.Ok(customer);
        });

        //deletes customer
        app.MapDelete("/customers/{id}", (Guid id) =>
        {
            var customer = customers.FirstOrDefault(c => c.GuId == id);
            if (customer == null)
                return Results.NotFound($"Customer with id {id} was not found.");
            customers.Remove(customer);
            return Results.NoContent();
        });

        //adds a payment method to a customer
        app.MapPost("/customers/{id}/paymentMethods", (Guid id, AddPaymentMethodDto dto) =>
        {
            var customer = customers.FirstOrDefault(c => c.GuId == id);
            if (customer == null)
                return Results.NotFound($"Customer with id {id} was not found.");

            var newPaymentMethod = new PaymentMethod(dto.Type, dto.LastFourDigits, dto.Expiry, dto.GatewayToken, id);
            AddPaymentMethodResult addResult = customer.AddPaymentMethod(newPaymentMethod);

            return addResult == AddPaymentMethodResult.Success
                ? Results.Created("Successfully added payment method", newPaymentMethod)
                : Results.BadRequest("Failed to add payment method");
        });

        //deletes a payment method from a customer
        app.MapDelete("/customers/{customerId}/paymentMethods/{paymentMethodId}", (Guid customerId, Guid paymentMethodId) =>
        {
            var customer = customers.FirstOrDefault(c => c.GuId == customerId);
            if (customer == null)
                return Results.NotFound($"Customer with id {customerId} was not found.");

            var paymentMethod = customer.PaymentMethods.FirstOrDefault(pm => pm.PaymentMethodId == paymentMethodId);
            if (paymentMethod == null)
                return Results.NotFound($"Payment method with id {paymentMethodId} was not found for customer {customerId}.");

            GenericResult removeResult = customer.RemovePaymentMethod(paymentMethodId);
            if (removeResult == GenericResult.Success) return Results.NoContent();
            return Results.BadRequest("Failed to remove payment method");
        });

    }
}
