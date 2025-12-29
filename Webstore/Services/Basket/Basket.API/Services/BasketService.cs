using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Grpc.Core;

namespace Basket.API.Services;

public class BasketService
{
    private readonly IBasketRepository _repository;
    private readonly ICouponGrpcService _couponGrpcService;
    private readonly ILogger<BasketService> _logger;

    public BasketService(IBasketRepository repository, ICouponGrpcService couponGrpcService, 
        ILogger<BasketService> logger)
    {
        _repository = repository;
        _couponGrpcService = couponGrpcService;
        _logger = logger;
    }

    public async Task<ShoppingCart?> GetBasketAsync(string username)
        => await _repository.GetBasketAsync(username) ?? new ShoppingCart(username);

    public async Task<ShoppingCart> UpdateBasketAsync(ShoppingCart basket)
    {
        foreach (var item in basket.Items)
        {
            try
            {
                var coupon = await _couponGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
            catch (RpcException e)
            {
                _logger.LogInformation("Error while retrieving coupon for item {ProductName}: {message}", 
                    item.ProductName, e.Message);
            }
        }
        return await _repository.UpdateBasketAsync(basket);
    }

    public Task DeleteBasketAsync(string username)
        => _repository.DeleteBasketAsync(username);
}