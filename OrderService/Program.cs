using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Entities;
using OrderService.Infrustructure;
using OrderService.Interfaces;
using OrderService.Messaging;
using OrderService.Repositories;
using OrderService.Resilience;
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

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContext<OrderDbContext>(options =>
                    options.UseInMemoryDatabase("OrderDb"));

                builder.Services.AddMassTransit(x =>
                {
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                });
            }
            else if (builder.Environment.IsStaging())
            {
                builder.Services.AddDbContext<OrderDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

                builder.Services.AddMassTransit(x =>
                {
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(builder.Configuration.GetConnectionString("MessageBrokerConnection"));
                        cfg.ConfigureEndpoints(context);
                    });
                });
            }
            else if (builder.Environment.IsProduction())
            {
                builder.Services.AddDbContext<OrderDbContext>(options =>
                    options.UseAzureSql(builder.Configuration.GetConnectionString("DbConnection")));

                builder.Services.AddMassTransit(x =>
                {
                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host(builder.Configuration.GetConnectionString("MessagingHost"));
                        cfg.ConfigureEndpoints(context);
                    });
                });
            };

            builder.Services.AddTransient<IValidator<OrderRequest>, OrderRequestValidator>();
            builder.Services.AddScoped<IOrderProducer, OrderProducer>();
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
                if (dbContext.Database.IsRelational())
                {
                    dbContext.Database.Migrate();
                }
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
