using AutoMapper;

namespace E_Commerce.Features.Products;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<CreateProductDto, Product>();
    }
}