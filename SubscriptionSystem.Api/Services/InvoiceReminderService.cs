using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Configuration;
using SubscriptionSystem.Data;
using SubscriptionSystem.Enums;
using SubscriptionSystem.Events;
using SubscriptionSystem.Services;
using System.Text.Json;

namespace SubscriptionSystem.Services;

public class InvoiceReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InvoiceReminderService> _logger;

    public InvoiceReminderService(IServiceProvider serviceProvider, ILogger<InvoiceReminderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scanning for unpaid invoices...");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var unpaidInvoices = await db.Invoices
                .Include(i => i.Subscription)
                .Where(i => i.Status != InvoiceStatus.Paid)
                .Where(i => i.DueDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var invoice in unpaidInvoices)
            {
                _logger.LogInformation("Unpaid invoice found: {InvoiceId} for Customer {CustomerId}, due on {DueDate}",
                    invoice.InvoiceId, invoice.CustomerId, invoice.DueDate);
            }
            if(unpaidInvoices.Count == 0)
            {
                _logger.LogInformation("No unpaid invoices found.");
            }
            
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}