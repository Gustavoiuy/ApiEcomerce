using Mapster;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Maping;

public static class MapsterConfig
{
    public static void RegisterMappings(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryDto>().TwoWays();
        config.NewConfig<Category, CreateCategoryDto>().TwoWays();

        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : string.Empty)
            .TwoWays();
        config.NewConfig<Product, CreateProductDto>().TwoWays();
        config.NewConfig<Product, UpdateProductDto>().TwoWays();

        config.NewConfig<Users, UserDto>().TwoWays();
        config.NewConfig<Users, CreateUserDto>().TwoWays();
        config.NewConfig<Users, UserLogginDto>().TwoWays();
        config.NewConfig<Users, UserLogginResponseDto>().TwoWays();
        config.NewConfig<ApplicationUser, UserDataDto>().TwoWays();
        config.NewConfig<ApplicationUser, UserDto>().TwoWays();
    }
}
