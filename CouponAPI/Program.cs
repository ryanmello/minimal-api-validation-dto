using AutoMapper;
using CouponAPI;
using CouponAPI.Data;
using CouponAPI.Models;
using CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

/* app.MapGet("api/coupon, () => { } returns a list of coupon objects
 * with a success status code of 200 upon creation of a coupon */

app.MapGet("api/coupon", (ILogger<Program> _logger) => {
	_logger.Log(LogLevel.Information, "Getting all coupons");
	return Results.Ok(CouponStore.Coupons);
}).Produces<IEnumerable<Coupon>>(200);

/* app.MapGet("api/coupon/{id:int}, () => { } returns a singele coupon.
 * the method requires a paramter id and will return the coupon object
 * at the give id address */

app.MapGet("api/coupon/{id:int}", (int id) => {
	return Results.Ok(CouponStore.Coupons.FirstOrDefault(c => c.Id == id));
}).Produces<Coupon>(200);

/* app.MapPost("api/coupon, () => { } creates a singele coupon object.
 * the method requires a coupon body. the method will validate the input.
 * inputs of an object with an id not equal to 0, with an empty name,
 * or a coupon that already exists, will throw a 400 error. all other 
 * sucessful inputs will create a coupon object and append it to the 
 * list of coupons */

app.MapPost("api/coupon", (IMapper _mapper, [FromBody] CouponCreateDTO coupon_C_DTO) => {
	/* checks if the name on the object being passed in null and if the coupon already exits */
	if (string.IsNullOrEmpty(coupon_C_DTO.Name))
	{
		return Results.BadRequest("Invalid Id or Coupon Name");
	}
	if (CouponStore.Coupons.FirstOrDefault(c => c.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
	{
		return Results.BadRequest("Coupon already exists.");
	}

	/* creates a new coupon object with the given DTO information */
	Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

	/* sets the current coupons id to one greater than the ID of the previous coupon */
	coupon.Id = CouponStore.Coupons.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
	CouponStore.Coupons.Add(coupon);

	/* creates a new coupon DTO from the coupon object and incremented ID */
	CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);
	return Results.Ok(couponDTO);
}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<CouponDTO>(201).Produces(400);

/*
 * 
 * 
 * 
 */

app.MapPut("api/coupon", () => {

});

/* 
 * 
 * 
 * 
 */

app.MapDelete("api/coupon/{id:int}", (int id) => {

});

app.UseHttpsRedirection();
app.Run();

