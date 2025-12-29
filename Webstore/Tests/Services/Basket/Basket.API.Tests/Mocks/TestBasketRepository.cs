using Basket.API.Entities;
using Basket.API.Repositories;

namespace Basket.API.Tests.Mocks;

public class TestBasketRepository : IBasketRepository
{
    private readonly Dictionary<string, ShoppingCart> items = new();

    public void AddTestCart(string username, ShoppingCart cart)
    {
        items.Add(username, cart);
    }
    
    public Task<ShoppingCart?> GetBasketAsync(string username)
    {
        return items.TryGetValue(username, out var basket) 
            ? Task.FromResult<ShoppingCart?>(basket) 
            : Task.FromResult<ShoppingCart?>(null);
    }

    public Task<ShoppingCart> UpdateBasketAsync(ShoppingCart basket)
    {
        items.Remove(basket.Username);
        items.Add(basket.Username, basket);
        return Task.FromResult(basket);
    }

    public Task DeleteBasketAsync(string username)
    {
        items.Remove(username);
        return Task.CompletedTask;
    }
}