using InventoryService.Business.Extensions;
using InventoryService.Business.Interfaces;
using InventoryService.Persistance.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistance(builder.Configuration);
builder.Services.AddBusinessServices(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
