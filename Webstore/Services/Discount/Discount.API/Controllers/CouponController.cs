using Microsoft.AspNetCore.Mvc;

using Discount.Common.DTOs;
using Discount.Common.Entities;
using Discount.Common.Repositories;

namespace Discount.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouponController: ControllerBase
{
    private readonly ICouponRepository _couponRepository;

    public CouponController(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
    }

    [HttpGet("{productName}")]
    [ProducesResponseType(typeof(Coupon), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CouponDTO>> GetDiscount(string productName)
    {
        var coupon = await _couponRepository.GetDiscountAsync(productName);
        return coupon is null ? NotFound() : Ok(coupon);
    }
    
    // Homework: Implement the rest
}