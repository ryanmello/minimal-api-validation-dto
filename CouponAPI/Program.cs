using AutoMapper;
using CouponAPI;
using CouponAPI.Data;
using CouponAPI.Models;
using CouponAPI.Models.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

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
	APIResponse response = new();
	_logger.Log(LogLevel.Information, "Getting all coupons");

	response.Result = CouponStore.Coupons;
	response.IsSucess = true;
	response.StatusCode = HttpStatusCode.OK;

	return Results.Ok(response);
}).Produces<APIResponse>(200);

/* app.MapGet("api/coupon/{id:int}, () => { } returns a singele coupon.
 * the method requires a paramter id and will return the coupon object
 * at the give id address */

app.MapGet("api/coupon/{id:int}", (int id) => {
	APIResponse response = new();
	response.Result = CouponStore.Coupons.FirstOrDefault(c => c.Id == id);
	response.IsSucess = true;
	response.StatusCode = HttpStatusCode.OK;

	return Results.Ok(response);
}).Produces<APIResponse>(200);

/* app.MapPost("api/coupon, () => { } creates a singele coupon object.
 * the method requires a coupon body. the method will validate the input.
 * inputs of an object with an id not equal to 0, with an empty name,
 * or a coupon that already exists, will throw a 400 error. all other 
 * sucessful inputs will create a coupon object and append it to the 
 * list of coupons */

app.MapPost("api/coupon", async (IMapper _mapper, IValidator<CouponCreateDTO> _validation, [FromBody] CouponCreateDTO coupon_C_DTO) => {

	APIResponse response = new();

	var validationResult = await _validation.ValidateAsync(coupon_C_DTO);
	/* checks if the name on the object being passed in is null and if the coupon already exits */
	if (!validationResult.IsValid)
	{
		response.ErrorMessages?.Add(validationResult.Errors.FirstOrDefault().ToString());
		return Results.BadRequest(response);
	}
	if (CouponStore.Coupons.FirstOrDefault(c => c.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
	{
		response.ErrorMessages?.Add("Coupon already exists.");
		return Results.BadRequest(response);
	}

	/* creates a new coupon object with the given DTO information */
	Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

	/* sets the current coupons id to one greater than the ID of the previous coupon */
	coupon.Id = CouponStore.Coupons.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
	CouponStore.Coupons.Add(coupon);

	/* creates a new coupon DTO from the coupon object and incremented ID */
	CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

	response.Result = couponDTO;
	response.IsSucess = true;
	response.StatusCode = HttpStatusCode.OK;
	return Results.Ok(response);
}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(200).Produces(400);

/* app.MapPut method takes in an integer id (represents the index of the coupon 
 * in the current list of coupons that will be updated), a new coupon DTO object,
 * an object validator that ensures the name and percent are valid, and a mapper (not used).
 * the method will update the coupon at the given index with the new coupon_C_DTO object */

app.MapPut("api/coupon", async (int id, CouponCreateDTO coupon_C_DTO, IValidator<CouponCreateDTO> _validation, IMapper _mapper) => {
	APIResponse response = new();

	/* ensure id exists already in coupons */
	if (!CouponStore.Coupons.Any(c => c.Id == id))
	{
		response.ErrorMessages.Add("Invalid Id input. Please enter a valid id");
		return Results.BadRequest(response);
	}

	/* validate the coupon_C_DTO object */
	var validationResult = await _validation.ValidateAsync(coupon_C_DTO);
	if (!validationResult.IsValid)
	{
		response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
		return Results.BadRequest(response);
	}

	/* get the coupon with the given id */
	Coupon currentCoupon = CouponStore.Coupons.FirstOrDefault(c => c.Id == id);

	/* updating the currentCoupon with the new coupon_C_DTO being passed in */
	currentCoupon.Name = coupon_C_DTO.Name;
	currentCoupon.Percent = coupon_C_DTO.Percent;
	currentCoupon.IsActive = coupon_C_DTO.IsActive;
	currentCoupon.LastUpdated = DateTime.Now;

	/* update the response code */
	response.Result = currentCoupon;
	response.IsSucess = true;
	response.StatusCode = HttpStatusCode.OK;

	return Results.Ok(response);
}).Produces<APIResponse>(200).Produces(400);

/* 
 * 
 * 
 * 
 */

app.MapDelete("api/coupon/{id:int}", (int id) => {
	APIResponse response = new();

	/* ensure id exists already in coupons */
	if (!CouponStore.Coupons.Any(c => c.Id == id))
	{
		response.ErrorMessages.Add("Enter a valid id");
		return Results.BadRequest(response);
	}

	/* remove the object at the given index */
	CouponStore.Coupons.RemoveAt(id - 1);

	response.IsSucess = true;
	response.StatusCode = HttpStatusCode.NoContent;
	return Results.Ok(response);
}).Produces<APIResponse>(200).Produces(400);

app.UseHttpsRedirection();
app.Run();

