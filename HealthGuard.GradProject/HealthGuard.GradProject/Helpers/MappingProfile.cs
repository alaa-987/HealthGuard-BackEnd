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
                     .ForMember(d => d.PictureUrl, o => o.MapFrom(s => $"{"https://localhost:7249"}/{s.PictureUrl}"))
                     .ReverseMap()
                     .ForMember(d => d.Category, o => o.Ignore());
            CreateMap<ProductCategory, ProductCategoryDto>()
                     .ForMember(d => d.PictureUrl, o => o.MapFrom(s => $"{"https://localhost:7249"}/{s.PictureUrl}"));
           // CreateMap<Product, ProductToReturnDto>()
           //.ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name))
           //.ForMember(d => d.PictureUrl, o => o.MapFrom(s => $"{"https://localhost:7249"}/{s.PictureUrl}"));
            CreateMap<ProductCreateDto, Product>().ReverseMap();
            CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
            CreateMap<BasketItemDto, BasketItem>().ReverseMap();
            CreateMap<CustomerWishlistDto, CustomerWishList>().ReverseMap();
            CreateMap<WishlistItemDto,WishListItem>().ReverseMap();
            CreateMap< Address, AddressDto>().ReverseMap();
            CreateMap<AddressDto, ShippingAddress>().ReverseMap();
            CreateMap<Order, OrderToReturnDto>()
             .ForMember(d => d.Total, o => o.MapFrom(s => s.GetTotal()))
             .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
             .ForMember(d => d.DeliveryMethodCost, o => o.MapFrom(s => s.DeliveryMethod.Cost))
             .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
             .ForMember(d => d.Total, o => o.MapFrom(s => s.GetTotal()))
             .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));
            CreateMap<OrderItem, OrderItemDto>()
                     .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                     .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                      .ForMember(d => d.PictureUrl, o => o.MapFrom(s => $"{"https://localhost:7249"}/{s.Product.PictureUrl}"));
            CreateMap<AppNurse, NurseDto>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Appointments, opt => opt.MapFrom(src => src.Appointments))
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
           .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
           .ForMember(d => d.PicUrl, o => o.MapFrom(s => $"{"https://localhost:7249"}/{s.PicUrl}"))
           .ReverseMap();
            CreateMap<Appointment, AppointmentDto>().ReverseMap();
            CreateMap<CreateNurseDto, AppNurse>().ReverseMap();
            CreateMap<AppNurse, NurseUpdateDto>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
           .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
           .ForMember(d => d.PicUrl, o => o.MapFrom(s => $"{"https://localhost:7249"}/{s.PicUrl}"))
           .ReverseMap();

            CreateMap<AppNurse, SpecificNusreDto>()
                .ForMember(dest => dest.PicUrl, opt => opt.MapFrom(s => $"{"https://localhost:7249"}/{s.PicUrl}"))
                .ReverseMap();

            CreateMap<AppUser, SpecificUserDto>().ReverseMap();

            CreateMap<Appointment, AllAppointmentDto>()
                .ForMember(dest => dest.AppNurse, opt => opt.MapFrom(src => src.AppNurse))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ReverseMap();


        }
    }
}
