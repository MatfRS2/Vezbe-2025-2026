using Basket.API.GrpcServices;
using Discount.GRPC;

namespace Basket.API.Tests.Mocks;

public class TestCouponGrpcService : ICouponGrpcService
{
    private readonly Dictionary<string, GetDiscountResponse> discounts = new();

    public void RegisterTestResponse(string productName, GetDiscountResponse cart)
    {
        discounts.Add(productName, cart);
    }

    public Task<GetDiscountResponse> GetDiscount(string productName)
    {
        if (discounts.TryGetValue(productName, out GetDiscountResponse? value)) {
            return Task.FromResult(value);
        }

        return Task.FromResult(new GetDiscountResponse());
    }

    public Task<GetRandomDiscountsResponse> GetRandomDiscounts(int numberOfDiscounts)
    {
        throw new NotImplementedException();
    }
}