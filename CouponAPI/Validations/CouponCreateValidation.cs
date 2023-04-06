using CouponAPI.Models.DTO;
using FluentValidation;
using AutoMapper;
using CouponAPI;
using CouponAPI.Data;
using CouponAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;

namespace CouponAPI.Validations
{
	public class CouponCreateValidation : AbstractValidator<CouponCreateDTO>
	{
		public CouponCreateValidation() 
		{
			RuleFor(model => model.Name).NotEmpty();
			RuleFor(model => model.Percent).InclusiveBetween(1, 100);
		}
	}
}
