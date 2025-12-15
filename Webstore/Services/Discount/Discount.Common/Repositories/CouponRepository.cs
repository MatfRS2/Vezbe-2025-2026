using AutoMapper;
using Dapper;
using Discount.Common.Data;
using Discount.Common.DTOs;
using Discount.Common.Entities;

namespace Discount.Common.Repositories;

public class CouponRepository : ICouponRepository
{
    private readonly ICouponContext _context;
    private readonly IMapper _mapper;

    public CouponRepository(ICouponContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<CouponDTO?> GetDiscountAsync(string productName)
    {
        using var connection = _context.GetConnection();
        const string sql = "SELECT * FROM Coupon WHERE ProductName = @ProductName";
        var coupon = await connection.QueryFirstOrDefaultAsync<Coupon?>(sql, new {ProductName = productName});
        return _mapper.Map<CouponDTO?>(coupon);
    }

    public async Task<bool> CreateDiscountAsync(CreateCouponDTO coupon)
    {
        using var connection = _context.GetConnection();
        var affected = await connection.ExecuteAsync(
            "INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
            new { coupon.ProductName, coupon.Description, coupon.Amount });
        return affected > 0;
    }

    public async Task<bool> UpdateDiscountAsync(UpdateCouponDTO coupon)
    {
        using var connection = _context.GetConnection();
        var affected = await connection.ExecuteAsync(
            "UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE ID=@Id" ,
            new { coupon.ProductName, coupon.Description, coupon.Amount, coupon.Id });
        return affected > 0;
    }

    public async Task<bool> DeleteDiscountAsync(string productName)
    {
        using var connection = _context.GetConnection();
        var affected = await connection.ExecuteAsync(
            "DELETE FROM Coupon WHERE ProductName = @ProductName",
            new { ProductName = productName });
        return affected != 0;
    }

    public async Task<IEnumerable<CouponDTO>> GetRandomDiscountsAsync(int numberOfDiscounts)
    {
        using var connection = _context.GetConnection();
        
        var coupons = await connection.QueryAsync<Coupon>(
            "SELECT * FROM Coupon");
        
        // Exact count - slow for big tables
        var couponCountRaw = await connection.ExecuteScalarAsync(
            "SELECT COUNT(*) AS exact_count FROM Coupon");
        // Estimate - extremely fast
        // var couponCountRaw = await connection.ExecuteScalarAsync(
        //     "SELECT reltuples::bigint AS estimate " +
        //     "FROM   pg_class " +
        //     "WHERE  oid = 'myschema.mytable'::regclass;");
        var couponCount = couponCountRaw as long? ?? 0L;
        if (couponCount < numberOfDiscounts)
            return _mapper.Map<IEnumerable<CouponDTO>>(coupons);

        var random = new Random();
        return _mapper.Map<IEnumerable<CouponDTO>>(
        coupons
                .Select(coupon => (Rand: random.Next(), Item: coupon))
                .OrderBy(obj => obj.Rand)
                .Select(obj => obj.Item)
                .Take(numberOfDiscounts)
        );
    }
}