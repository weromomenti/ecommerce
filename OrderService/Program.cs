using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderService.Controllers;
using OrderService.Entities;
using OrderService.Infrustructure;
using OrderService.Interfaces;
using OrderService.Repositories;
using OrderService.Resilience;
using Polly.CircuitBreaker;
using Serilog;
using System.Text.Json.Serialization;

namespace OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<OrderDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddTransient<IValidator<OrderRequest>, OrderRequestValidator>();
            builder.Services.AddSingleton<IOrderProducer, OrderProducer>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddSingleton(ResiliencePolicyHelper.GetCircuitBreakerPolicy());

            builder.Services.AddAutoMapper(configAction => configAction.AddProfile<OrderMapper>());

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHealthChecks();


            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                dbContext.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            // Enable Swagger for all environments (not just Development) when running in Docker
            app.UseSwagger();
            app.UseSwaggerUI();

            // Remove HTTPS redirection for Docker containers
            // app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}
