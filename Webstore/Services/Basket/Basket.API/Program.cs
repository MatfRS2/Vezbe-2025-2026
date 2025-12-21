using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Discount.GRPC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddGrpcClient<CouponProtoService.CouponProtoServiceClient>(
    options => options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));
builder.Services.AddScoped<CouponGrpcService>();
builder.Services.AddControllers();

builder.Services.AddStackExchangeRedisCache(
    opts => {
        opts.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString"); 
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
