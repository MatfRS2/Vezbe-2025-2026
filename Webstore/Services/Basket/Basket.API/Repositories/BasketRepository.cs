using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _cache;

    public BasketRepository(IDistributedCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<ShoppingCart?> GetBasketAsync(string username)
    {
        var basket = await _cache.GetStringAsync(username);
        if (string.IsNullOrEmpty(basket))
            return null;
        return JsonConvert.DeserializeObject<ShoppingCart>(basket);
    }

    public async Task<ShoppingCart> UpdateBasketAsync(ShoppingCart basket)
    {
        var basketString = JsonConvert.SerializeObject(basket);
        await _cache.SetStringAsync(basket.Username, basketString);
        var updatedBasket = await GetBasketAsync(basket.Username);
        return updatedBasket ?? basket;
    }

    public async Task DeleteBasketAsync(string username)
    {
        await _cache.RemoveAsync(username);
    }
}