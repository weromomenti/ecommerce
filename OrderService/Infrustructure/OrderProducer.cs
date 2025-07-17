using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using OrderService.Interfaces;
using OrderService.Entities;

namespace OrderService.Infrustructure
{
    public class OrderProducer(IConfiguration configuration, ILogger<OrderProducer> logger) : IOrderProducer, IDisposable, IAsyncDisposable
    {
        private IConnection _connection;
        private IChannel _channel;

        private readonly IConnectionFactory _factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"],
            Port = int.Parse(configuration["RabbitMQ:Port"]),
        };

        public async Task SendOrderCreatedMessageAsync(OrderEntity order)
        {
            await SendMessageAsync(order, EventType.Created);
        }

        public async Task SendOrderUpdatedMessageAsync(OrderEntity order)
        {
            await SendMessageAsync(order, EventType.Updated);
        }

        public async Task SendOrderDeletedMessageAsync(OrderEntity order)
        {
            await SendMessageAsync(order, EventType.Deleted);
        }

        private async Task SendMessageAsync(OrderEntity order, EventType eventType)
        {
            await EnsureConnectionAndChannelAsync();

            var orderJson = JsonSerializer.Serialize(order);
            var message = Encoding.UTF8.GetBytes(orderJson);

            await _channel.BasicPublishAsync(
                exchange: "orders",
                routingKey: "orders." + eventType.ToString().ToLower(),
                mandatory: false,
                basicProperties: new BasicProperties(),
                body: message,
                cancellationToken: CancellationToken.None
            );
        }

        private async Task EnsureConnectionAndChannelAsync()
        {
            if (_channel?.IsOpen == true)
            {
                return;
            }

            logger.LogInformation("Re-establishing RabbitMQ connection and channel...");

            _connection?.Dispose();
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            var queue = await _channel.QueueDeclareAsync(
                queue: "orders",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await _channel.ExchangeDeclareAsync(
                exchange: "orders",
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: queue,
                exchange: "orders",
                routingKey: "orders.*");
        }

        public void Dispose()
        {
            logger.LogInformation("Disposing OrderProducer...");
            _connection?.Dispose();
            _channel?.Dispose();
            logger.LogInformation("OrderProducer disposed.");
        }

        public async ValueTask DisposeAsync()
        {
            logger.LogInformation("Disposing OrderProducer...");
            await _connection.DisposeAsync();
            await _channel.DisposeAsync();
            logger.LogInformation("OrderProducer disposed.");
        }
    }
}
