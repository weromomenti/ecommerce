namespace InventoryService.Hosting.RabbitMQ
{
    public interface IOrderConsumer : 
    {
        Task StartConsumingAsync();
    }
}