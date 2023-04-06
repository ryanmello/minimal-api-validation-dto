using CouponAPI.Data;
using CouponAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapGet("api/coupon", () => {
	return Results.Ok(CouponStore.Coupons);
}).Produces<IEnumerable<Coupon>>(200);

app.MapGet("api/coupon/{id:int}", (int id) => {
	return Results.Ok(CouponStore.Coupons.FirstOrDefault(c => c.Id == id));
}).Produces<Coupon>(200);


app.MapPost("api/coupon", ([FromBody] Coupon coupon) => {
	if (coupon.Id != 0 || string.IsNullOrEmpty(coupon.Name))
	{
		return Results.BadRequest("Invalid Id or Coupon Name");
	}

	if (CouponStore.Coupons.FirstOrDefault(c => c.Name.ToLower() == coupon.Name.ToLower()) != null)
	{
		return Results.BadRequest("Coupon already exists.");
	}

	coupon.Id = CouponStore.Coupons.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
	CouponStore.Coupons.Add(coupon);
	return Results.Ok(coupon);
}).Produces<Coupon>(201).Produces(400);

app.MapPut("api/coupon", () => {

});

app.MapDelete("api/coupon/{id:int}", (int id) => {

});

app.UseHttpsRedirection();
app.Run();

