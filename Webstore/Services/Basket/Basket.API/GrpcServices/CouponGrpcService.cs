using Discount.GRPC;

namespace Basket.API.GrpcServices;

public class CouponGrpcService : ICouponGrpcService
{
    private readonly CouponProtoService.CouponProtoServiceClient _couponProtoServiceClient;

    public CouponGrpcService(CouponProtoService.CouponProtoServiceClient couponProtoServiceClient)
    {
        _couponProtoServiceClient = couponProtoServiceClient ??
            throw new ArgumentNullException(nameof(couponProtoServiceClient));
    }

    public async Task<GetDiscountResponse> GetDiscount(string productName)
    {
        var request = new GetDiscountRequest
        {
            ProductName = productName
        };
        return await _couponProtoServiceClient.GetDiscountAsync(request);
    }

    public async Task<GetRandomDiscountsResponse> GetRandomDiscounts(int numberOfDiscounts)
    {
        var request = new GetRandomDiscountsRequest
        {
            NumberOfDiscounts = numberOfDiscounts
        };
        return await _couponProtoServiceClient.GetRandomDiscountsAsync(request);
    }
}