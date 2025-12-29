using Basket.API.Entities;
using Basket.API.Services;
using Basket.API.Tests.Mocks;
using Discount.GRPC;

namespace Basket.API.Tests;

public class Tests
{
    private TestBasketRepository repo;
    private TestCouponGrpcService grpc;
    private BasketService service;
    
    [SetUp]
    public void Setup()
    {
        repo = new TestBasketRepository();
        grpc = new TestCouponGrpcService();
        service = new BasketService(repo, grpc, new TestLogger());
    }

    [Test]
    public async Task TestGetBasketNotCached()
    {
        const string user = "John";
        var cart = await service.GetBasketAsync(user);
        Assert.That(cart, Is.Not.Null);
        Assert.That(cart.Username, Is.EqualTo(user));
        Assert.That(cart.Items, Is.Empty);
    }
    
    [Test]
    public async Task TestGetBasketCached()
    {
        string user = "John";
        repo.AddTestCart(user, new ShoppingCart(user)
        {
            Items = new List<ShoppingCartItem>
            {
                new()
                {
                    ProductId = "IPXB123",
                    ProductName = "IPhone X",
                    Color = "Black",
                    Price = 3000M,
                    Quantity = 1,
                },
                new()
                {
                    ProductId = "SG12B123",
                    ProductName = "Samsung Galaxy 12",
                    Color = "Black",
                    Price = 2500M,
                    Quantity = 2,
                }
            }
        });
        
        var cart = await service.GetBasketAsync(user);
        Assert.That(cart, Is.Not.Null);
        Assert.That(cart.Username, Is.EqualTo(user));
        Assert.That(cart.Items, Has.Exactly(2).Items);
    }
    
    [Test]
    public async Task TestUpdateBasketWithAppliedDiscount()
    {
        const string user = "John";
        var updatedCart = new ShoppingCart(user)
        {
            Items = new List<ShoppingCartItem>
            {
                new()
                {
                    ProductId = "IPXB123",
                    ProductName = "IPhone X",
                    Color = "Black",
                    Price = 3000M,
                    Quantity = 1,
                },
                new()
                {
                    ProductId = "SG12B123",
                    ProductName = "Samsung Galaxy 12",
                    Color = "Black",
                    Price = 2500M,
                    Quantity = 2,
                }
            }
        };
        
        grpc.RegisterTestResponse("IPhone X", new GetDiscountResponse()
        {
            ProductName = "IPhone X",
            Amount = 1000,
            Id = 0,
            ProductDescription = "?",
        });
        
        var cart = await service.GetBasketAsync(user);
        Assert.That(cart, Is.Not.Null);
        Assert.That(cart.Username, Is.EqualTo(user));
        Assert.That(cart.Items, Is.Empty);

        var rv = await service.UpdateBasketAsync(updatedCart);
        Assert.That(rv, Is.EqualTo(updatedCart));
        
        cart = await service.GetBasketAsync(user);
        Assert.That(cart, Is.Not.Null);
        Assert.That(cart.Username, Is.EqualTo(user));
        Assert.That(cart.Items, Has.Exactly(2).Items);
        Assert.That(cart.TotalPrice, Is.EqualTo(7000M));
    }
}