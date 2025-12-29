using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Services;
using Discount.GRPC;

namespace Basket.API.Extensions;

public static class BasketExtensions
{
    public static IServiceCollection AddBasketServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddGrpcClient<CouponProtoService.CouponProtoServiceClient>(
            options => options.Address = new Uri(configuration.GetValue<string>("GrpcSettings:DiscountUrl")));
        services.AddScoped<ICouponGrpcService, CouponGrpcService>();
        services.AddScoped<BasketService>();
        services.AddStackExchangeRedisCache(
            opts => {
                opts.Configuration = configuration.GetValue<string>("CacheSettings:ConnectionString"); 
            }
        );
        return services;
    }
}