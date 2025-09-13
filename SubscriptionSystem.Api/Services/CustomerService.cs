namespace SubscriptionSystem.Services;

using SubscriptionSystem.Data;
using SubscriptionSystem.Models;
using SubscriptionSystem.Results;
using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Enums;
using Microsoft.VisualBasic;
using SubscriptionSystem.Dtos;

public class CustomerService
{
    private readonly AppDbContext _db;

    public CustomerService(AppDbContext db)
    {
        _db = db;
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