using Discount.GRPC;

namespace Basket.API.GrpcServices;

public interface ICouponGrpcService
{
    Task<GetDiscountResponse> GetDiscount(string productName);
    Task<GetRandomDiscountsResponse> GetRandomDiscounts(int numberOfDiscounts);
}