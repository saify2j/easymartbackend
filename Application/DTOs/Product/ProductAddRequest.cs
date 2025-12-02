using FluentValidation;
using System;

namespace EasyMart.API.Application.DTOs.Product
{
    public class ProductAddRequest
    {
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
    }
    public class ProductAddRequestValidator : AbstractValidator<ProductAddRequest>
    {
        public ProductAddRequestValidator()
        {
            RuleFor(x => x.ProductName).NotEmpty();
            RuleFor(x => x.Price)
            .InclusiveBetween(0.01m, 10_000_000m)
            .WithMessage("Price must be between $0.01 and $10,000,000");
        }
    }
}
