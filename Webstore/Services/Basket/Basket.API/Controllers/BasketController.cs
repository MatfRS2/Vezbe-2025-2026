using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BasketController: ControllerBase
{
    private readonly IBasketRepository _repository;
    private readonly CouponGrpcService _couponGrpcService;
    private readonly ILogger<BasketController> _logger;

    public BasketController(IBasketRepository basketRepository, CouponGrpcService couponGrpcService,
        ILogger<BasketController> logger)
    {
        _repository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _couponGrpcService = couponGrpcService ?? throw new ArgumentNullException(nameof(couponGrpcService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{username}")]
    [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
    {
        var basket = await _repository.GetBasketAsync(username);
        return Ok(basket ?? new ShoppingCart(username));
    }
    
    [HttpPut]
    [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
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
                _logger.LogInformation("Error while retrieving coupon for item {ProductName}: {message}", item.ProductName, e.Message);
            }
        }
        return Ok(await _repository.UpdateBasketAsync(basket));
    }

    [HttpDelete("{username}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteBasket(string username)
    {
        await _repository.DeleteBasketAsync(username);
        return Ok();
    }
}