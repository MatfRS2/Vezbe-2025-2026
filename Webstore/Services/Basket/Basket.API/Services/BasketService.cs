using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using Grpc.Core;
using MassTransit;

namespace Basket.API.Services;

public class BasketService
{
    private readonly IBasketRepository _repository;
    private readonly ICouponGrpcService _couponGrpcService;
    private readonly ILogger<BasketService> _logger;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public BasketService(IBasketRepository repository, ICouponGrpcService couponGrpcService, 
        ILogger<BasketService> logger, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _couponGrpcService = couponGrpcService;
        _logger = logger;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
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

    public async Task<bool> CheckoutAsync(BasketCheckout basketCheckout)
    {
        var basket = await _repository.GetBasketAsync(basketCheckout.BuyerUsername);
        if (basket is null)
        {
            return false;
        }
        
        var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        await _publishEndpoint.Publish(eventMessage);
        await _repository.DeleteBasketAsync(basketCheckout.BuyerUsername);
        
        return true;
    }
}