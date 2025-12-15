using Discount.Common.DTOs;

namespace Discount.Common.Repositories;

public interface ICouponRepository
{
    Task<CouponDTO?> GetDiscountAsync(string productName);
    Task<bool> CreateDiscountAsync(CreateCouponDTO coupon);
    Task<bool> UpdateDiscountAsync(UpdateCouponDTO coupon);
    Task<bool> DeleteDiscountAsync(string productName);
    Task<IEnumerable<CouponDTO>> GetRandomDiscountsAsync(int numberOfDiscounts);
}