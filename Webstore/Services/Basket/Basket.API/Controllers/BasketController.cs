using Basket.API.Entities;
using Basket.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BasketController: ControllerBase
{
    private readonly BasketService _service;

    public BasketController(BasketService service)
    {
        _service = service;
    }

    [HttpGet("{username}")]
    [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
    {
        return Ok(await _service.GetBasketAsync(username));
    }
    
    [HttpPut]
    [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
    {
        return Ok(await _service.UpdateBasketAsync(basket));
    }

    [HttpDelete("{username}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteBasket(string username)
    {
        await _service.DeleteBasketAsync(username);
        return Ok();
    }
    
    [Route("[action]")]
    [HttpPost]
    [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        bool success = await _service.CheckoutAsync(basketCheckout);
        return success ? Accepted() : BadRequest();
    }
}