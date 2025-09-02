using InventoryService.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Hosting.RabbitMQ
{
    public sealed record OrderContract
    {
        public OrderDto order;

        public EventType eventType;
    }
}
