using AutoMapper;
using Discount.Common.Repositories;
using Grpc.Core;

namespace Discount.GRPC.Services;

public class CouponService: CouponProtoService.CouponProtoServiceBase
{
    private readonly ICouponRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<CouponService> _logger;

    public CouponService(ICouponRepository couponRepository, IMapper mapper, ILogger<CouponService> logger)
    {
        _repo = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<GetDiscountResponse> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await _repo.GetDiscountAsync(request.ProductName);
        _logger.LogInformation("Get discount for product: {ProductName}", coupon?.ProductName);
        return _mapper.Map<GetDiscountResponse>(coupon);
    }

    public override async Task<GetRandomDiscountsResponse> GetRandomDiscounts(GetRandomDiscountsRequest request, ServerCallContext context)
    {
        var coupons = await _repo.GetRandomDiscountsAsync(request.NumberOfDiscounts);

        var response = new GetRandomDiscountsResponse();
        response.Coupons.AddRange(_mapper.Map<IEnumerable<GetRandomDiscountsResponse.Types.Coupon>>(coupons));
        response.TotalAmount = response.Coupons.Sum(coupon => coupon.Amount);

        _logger.LogInformation("Retrieved {numberOfDiscounts} random coupon(s) of total value {amount}", 
            request.NumberOfDiscounts, response.TotalAmount);

        return response;
    }
}