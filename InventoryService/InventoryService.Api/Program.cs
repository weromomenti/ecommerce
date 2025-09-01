using InventoryService.Business.Extensions;
using InventoryService.Business.Interfaces;
using InventoryService.Persistance.Extensions;
using InventoryService.Persistance.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistance(builder);
builder.Services.AddBusinessServices(builder);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }
    InventoryDataSeeder.Seed(dbContext);
}


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.MapGet("/GetProductHistory", async (string productName, IInventoryManagementService inventoryService) =>
{
    var productHistory = await inventoryService.GetProductHistoryAsync(productName);
    return Results.Ok(productHistory);
});

app.MapGet("/GetAllProducts", async (IInventoryManagementService inventoryService) =>
{
    var products = await inventoryService.GetAllProductsAsync();
    return Results.Ok(products);
});

app.Run();
