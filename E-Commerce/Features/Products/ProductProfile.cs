using AutoMapper;
using E_Commerce.Features.Cart;
using E_Commerce.Features.Order;

namespace E_Commerce.Features.Products;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<CreateProductDto, Product>();

        // AutoMapper converts database entities into DTOs and copies the values into the right properties automatically.
        // Set CartItemDto.ProductName using CartItem.Product.Name.
        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price));

        CreateMap<Cart.Cart, CartDto>()
            .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<Order.Order, OrderDto>();
    }
}