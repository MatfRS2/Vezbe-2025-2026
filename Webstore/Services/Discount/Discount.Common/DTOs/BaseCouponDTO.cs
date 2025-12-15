namespace Discount.Common.DTOs;

public class BaseCouponDTO
{
    public required string ProductName { get; set; }
    public required int Amount { get; set; }
    public required string Description { get; set; }
    
}