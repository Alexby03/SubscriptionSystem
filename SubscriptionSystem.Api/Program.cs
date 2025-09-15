using SubscriptionSystem.Data;
using Microsoft.EntityFrameworkCore;
using SubscriptionSystem.Services;
using SubscriptionSystem.Events;
using SubscriptionSystem.Outbox;
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

//dotnet run --urls "http://0.0.0.0:5034"

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(9, 4, 0))
    )
);
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<SubscriptionService>();
builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<PaymentService>();

//Dispatcher to azure bus
builder.Services.AddSingleton<IEventDispatcher>(sp =>
    new AzureServiceBusDispatcher(
        builder.Configuration.GetConnectionString("AzureServiceBus")!,
        builder.Configuration["ServiceBus:TopicName"]!
    )
);


//outbox handler
builder.Services.AddHostedService<OutboxWorker>(); 

//connection to azure bus
builder.Services.AddSingleton(sp => 
    new ServiceBusClient(builder.Configuration.GetConnectionString("AzureServiceBus")!));

//azure bus subscription created handler
builder.Services.AddHostedService(sp => 
    new AzureServiceBusWorker(
        sp,
        sp.GetRequiredService<ILogger<AzureServiceBusWorker>>(),
        sp.GetRequiredService<ServiceBusClient>(),
        builder.Configuration["ServiceBus:TopicName"]!,
        "all-events-sub"
    )
);

//azure bus customer created handler
builder.Services.AddHostedService(sp =>
    new AzureServiceBusWorker(
        sp,
        sp.GetRequiredService<ILogger<AzureServiceBusWorker>>(),
        sp.GetRequiredService<ServiceBusClient>(),
        builder.Configuration["ServiceBus:TopicName"]!,
        "all-events-sub"
    )
);

builder.Services.AddScoped<IEventHandler<CustomerCreatedEvent>, CustomerCreatedEventHandler>();
builder.Services.AddScoped<IEventHandler<SubscriptionCreatedEvent>, SubscriptionCreatedEventHandler>();
builder.Services.AddHostedService<OutboxWorker>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); //creates tables if they don’t exist
}

app.MapCustomerEndpoints();
app.MapSubscriptionEndpoints();
app.MapPlanEndpoints();
app.MapInvoiceEndpoints();
app.MapPaymentEndpoints();

app.Run();