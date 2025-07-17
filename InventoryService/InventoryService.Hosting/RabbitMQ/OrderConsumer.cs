using System.Text;
using System.Text.Json;
using AutoMapper;
using InventoryService.Business.Entities;
using InventoryService.Business.Interfaces;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace InventoryService.Hosting.RabbitMQ
{
    public class OrderConsumer(
        ILogger<OrderConsumer> logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory) : IOrderConsumer, IDisposable
    {
        private readonly IConnectionFactory _factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"],
            Port = int.Parse(configuration["RabbitMQ:Port"]),
        };

        private IConnection? _connection;
        private IChannel? _channel;
        private string? _consumerTag;

        public async Task StartConsumingAsync()
        {
            await EnsureConnection();

            await _channel.ExchangeDeclareAsync(
                exchange: "orders",
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            var queue = await _channel.QueueDeclareAsync(
                queue: "orders",
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: queue,
                exchange: "orders",
                routingKey: "orders.*");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            _consumerTag = await _channel.BasicConsumeAsync(
                queue: queue,
                autoAck: true,
                consumer: consumer);

            consumer.ReceivedAsync += OnMessageReceived;

            logger.LogInformation("Started consuming messages from 'orders' queue with consumer tag: {ConsumerTag}", _consumerTag);
        }

        private async Task EnsureConnection()
        {
            var retryPolicy = Policy
                .Handle<BrokerUnreachableException>()
                .WaitAndRetryAsync(5, _ => TimeSpan.FromSeconds(2));

            _connection = await retryPolicy.ExecuteAsync(() => _factory.CreateConnectionAsync());
            _channel = await _connection.CreateChannelAsync();
        }

        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            logger.LogDebug("Received message: {Message}", message);
            var order = JsonSerializer.Deserialize<OrderDto>(message);
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var inventoryManagementService = scope.ServiceProvider.GetRequiredService<IInventoryManagementService>();
                switch (ea.RoutingKey)
                {
                    case "orders.created":
                        logger.LogInformation("Processing created order message");
                        await inventoryManagementService.ProcessCreateOrderAsync(order);
                        break;

                    case "orders.updated":
                        logger.LogInformation("Processing updated order message");
                        await inventoryManagementService.ProcessUpdateOrderAsync(order);
                        break;

                    case "orders.deleted":
                        logger.LogInformation("Processing deleted order message");
                        await inventoryManagementService.ProcessCancelOrderAsync(order);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            logger.LogDebug("Message acknowledged for order ID: {OrderId}", order.Id);
        }

        private async Task StopConsumingAsync()
        {
            await _channel?.BasicCancelAsync(_consumerTag);
            logger.LogInformation("Consumer cancelled: {ConsumerTag}", _consumerTag);
        }

        public void Dispose()
        {
            if (_consumerTag != null)
            {
                StopConsumingAsync().GetAwaiter().GetResult();
            }

            _channel?.Dispose();
            _connection?.Dispose();

            logger.LogInformation("RabbitMQOrderConsumer disposed");
        }
    }
}