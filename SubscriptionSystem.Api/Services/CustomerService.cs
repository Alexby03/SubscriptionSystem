namespace SubscriptionSystem.Services;

using SubscriptionSystem.Data;
using SubscriptionSystem.Entities;
using SubscriptionSystem.Results;
using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Dtos;
using SubscriptionSystem.Events;
using SubscriptionSystem.Outbox;
using System.Text.Json;

public class CustomerService
{
    private readonly AppDbContext _db;
    private readonly IEventDispatcher _dispatcher;

    public CustomerService(AppDbContext db, IEventDispatcher dispatcher)
    {
        _db = db;
        _dispatcher = dispatcher;
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        return await _db.Customers.ToListAsync();
    }

    public async Task<Customer?> GetCustomerByEmailAsync(string email)
    {
        return await _db.Customers.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<GenericResult> CreateCustomerAsync(CreateCustomerDto dto)
    {
        var customer = new Customer(dto.Name, dto.Email, dto.BillingAddress);
        try
        {
            _db.Customers.Add(customer);

            var @event = new CustomerCreatedEvent(customer.CustomerId, customer.Name, customer.Email);
            var outboxEvent = new OutboxEvent()
            {
                EventType = nameof(CustomerCreatedEvent),
                Payload = JsonSerializer.Serialize(@event)
            };
            _db.OutboxEvents.Add(outboxEvent);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("Duplicate entry") == true)
        {
            return GenericResult.Duplicate;
        }
        return GenericResult.Success;
    }

    public async Task<Customer?> UpdateCustomerAsync(Guid id, CreateCustomerDto dto)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
        if (customer == null)
            return null;
        customer.Update(dto.Name, dto.Email, dto.BillingAddress);
        await _db.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer?> MarkCustomerInactiveAsync(Guid id)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
        if (customer == null)
            return null;
        customer.MarkInactive();
        await _db.SaveChangesAsync();
        return customer;
    }

}