using AutoMapper;
using E_Commerce.DTOs;
using E_Commerce.Models;

namespace E_Commerce.Mapper;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<CreateProductDto, Product>();
    }
}