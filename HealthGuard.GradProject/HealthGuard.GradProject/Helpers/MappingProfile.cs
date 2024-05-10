using AutoMapper;
using HealthGuard.Core.Entities;
using HealthGuard.Core.Entities.Identity;
using HealthGuard.Core.Entities.Order;
using HealthGuard.GradProject.DTO;

namespace HealthGuard.GradProject.Helpers
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductToReturnDto>()
                     .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name))
                     .ForMember(d => d.PictureUrl, o => o.MapFrom(s => $"{"https://localhost:7249"}/{s.PictureUrl}"));
            CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
            CreateMap<BasketItemDto, BasketItem>().ReverseMap();
            CreateMap< Address, AddressDto>().ReverseMap();
            CreateMap<AddressDto, ShippingAddress>().ReverseMap();
            CreateMap<Order, OrderToReturnDto>()
                   .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                   .ForMember(d => d.DeliveryMethodCost, o => o.MapFrom(s => s.DeliveryMethod.Cost));
            CreateMap<OrderItem, OrderItemDto>()
                     .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                     .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                     .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.Product.PictureUrl))
                     .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemUrlResolver>());
        }
    }
}
