using AutoMapper;
using Ecommerce.Library.Dtos;
using OrderService.Entities;

namespace OrderService.Infrustructure
{
    public class OrderMapper : Profile
    {
        public OrderMapper()
        {
            CreateMap<OrderEntity, OrderDto>().ReverseMap();
        }
    }

    public static class MapperExtensions
    {
        public static OrderDto ToDto(this OrderEntity order)
        {
            return new OrderDto
            {
                Id = order.Id,
                ProductName = order.ProductName,
                Quantity = order.Quantity,
            };
        }
        public static OrderEntity ToEntity(this OrderDto orderDto)
        {
            return new OrderEntity
            {
                Id = orderDto.Id,
                ProductName = orderDto.ProductName,
                Quantity = orderDto.Quantity,
            };
        }
    }
}
