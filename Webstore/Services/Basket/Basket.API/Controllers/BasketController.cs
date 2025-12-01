using Basket.API.Entities;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BasketController: ControllerBase
{
    private readonly IBasketRepository _repository;

    public BasketController(IBasketRepository basketRepository)
    {
        _repository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
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